using ChatWeb3.Data;
using ChatWeb3.Models;
using ChatWeb3.Models.OutputModels;
using ChatWeb3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;

namespace ChatWeb3.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        // to keep track of online users dict key-value pair
        // key - userId     value - connectionId
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();        //dictonary to store online users userid and connection id
        Response response = new Response();
        private readonly ChatAppDbContext DbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatHub> _logger;
        private readonly IHubService _hubService;

        public ChatHub(ChatAppDbContext dbContext, IConfiguration configuration,ILogger<ChatHub> logger, IHubService hubService)
        {
            _configuration = configuration;
            DbContext = dbContext;
            _logger = logger;
            _hubService = hubService;
        }

        public async override Task<Task> OnConnectedAsync()
        {
            try
            {
                _logger.LogInformation("new user connected");
                string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;               //get user id
                await Clients.Caller.SendAsync("UserConnected", Context.ConnectionId);              // reply back to connection to confirm his connection to socket
                User user = DbContext.Users.Find(new Guid(userId))!;
                await ToggleUserOnlineState(user, true);
                await refresh();
                AddUserConnectionId(userId);                                                // add user to dictionary of connected users
            }
            catch (Exception ex)
            {
                _logger.LogError("internal server error: ", ex.Message);
            }

            return base.OnConnectedAsync();
        }

        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("user disconnected");
            string? userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid);
            RemoveUserFromList(userId!);                    //remove user from connected users dictionary
            await refresh();
            Guid id = new Guid(userId!);
            User user = DbContext.Users.Find(id)!;
            await ToggleUserOnlineState(user,false);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(InputMessage msg)
        {
            _logger.LogInformation("SendMessage method started ");
            string senderId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            Guid senderGuid = new Guid(senderId);
            if (senderId == string.Empty || msg.chatId == string.Empty || senderId == null || msg.chatId == null)
            {
                return;
            }

            //Console.WriteLine(msg);
            Message message = new Message(msg,senderGuid);
            _logger.LogInformation(message.ToString());

            await DbContext.Messages.AddAsync(message);
            User? user = DbContext.Users.Find(senderGuid);
            if (user != null)
            {
                user.lastActive = DateTime.Now;
                user.isOnline = true;
            }
            if(msg.isGroup)
            {
                Guid groupId = DbContext.ChatMappings.Find(new Guid(msg.chatId.ToLower()))!.receiverId;
                List<Guid> receivers = DbContext.ChatMappings.Where(s => s.receiverId == groupId).Select(s => s.senderId).ToList();
                List<User> grpMembers =  DbContext.Users.Where(s => receivers.Contains(s.id)).ToList();
                foreach (var member in grpMembers)
                {
                    ChatMappings chat = DbContext.ChatMappings.Where(s => s.senderId == member.id).FirstOrDefault()!;
                    chat.datetime = DateTime.Now;
                    await DbContext.SaveChangesAsync();
                    OutputMessage sendMsg = new OutputMessage(message);
                    await SendMessageHelper(sendMsg, member.id);
                }
                await DbContext.SaveChangesAsync();
                return;
            }
            var chatmap = DbContext.ChatMappings.Find(new Guid(msg.chatId.ToLower()))!;
            if(chatmap != null)
            {
                chatmap.datetime = DateTime.Now;
                await DbContext.SaveChangesAsync();
                //ChatMappings chat = DbContext.ChatMappings.Find(msg.chatId)!;
                Guid receiverId = (chatmap.receiverId != new Guid(senderId))?chatmap.receiverId:chatmap.senderId;
                OutputMessage sendMsg = new OutputMessage(message);
                await Clients.Caller.SendAsync("ReceivedMessage", sendMsg);
                await SendMessageHelper(sendMsg, receiverId);
            }
            return;
        }

        private async Task SendMessageHelper(OutputMessage message,Guid receiverId)
        {
            string receiverConnectionId = GetConnectionIdByUser(receiverId.ToString().ToLower());
            var fileName = message.pathToFileAttachment!.Split("/").Last();

            //await Clients.Caller.SendAsync("ReceivedMessage", sendMsg);
            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivedMessage", message);
            }
            //await refresh();
        }

        public async Task CreateChat(string ConnectToId)
        {
            Console.WriteLine("createChat fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            string ReceiverId = GetConnectionIdByUser(ConnectToId.ToLower());
            if (userId.ToLower() == ConnectToId.ToLower())
            {
                await Clients.Caller.SendAsync("ChatCreated", "Can't Connect to yourself");
                return;
            }
            var res = await _hubService.AddChat(userId,ConnectToId);
            await Clients.Clients(ReceiverId).SendAsync("ChatCreated", res);
            await Clients.Caller.SendAsync("ChatCreated", res);
        }
        public async Task CreateGroup(string name, string description,string pathToPic)
        {
            Console.WriteLine("createGroup fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            
            var res = await _hubService.AddGroup(userId, name, description, pathToPic);
            //await Clients.Client(ReceiverId).SendAsync("GroupCreated", res);
            await Clients.Caller.SendAsync("GroupCreated", res);
        }

        public async Task AddMemberToGroup(string groupId, string userToAdd)
        {
            Console.WriteLine("add user to grp fxn called");
            string adminId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            //Guid adminGuid = Guid.Parse(adminId);
            var res = await _hubService.AddUserToGroup(adminId, groupId, userToAdd);
            if (res.statusCode == 200)
            {
                string ReceiverId = GetConnectionIdByUser(userToAdd.ToLower());
                await Clients.Client(ReceiverId).SendAsync("UserAddedToGroup", res);
            }
            await Clients.Caller.SendAsync("UserAddedToGroup", res);
        }

        public async Task RemoveMemberFromGroup(string groupId, string userToRemove)
        {
            Console.WriteLine("remove user from grp fxn called");
            string adminId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            Guid adminGuid = Guid.Parse(adminId);
            var res = await _hubService.RemoveUserFromGroup(adminId, groupId, userToRemove);
            if (res.statusCode == 200)
            {
                string ReceiverId = GetConnectionIdByUser(userToRemove.ToLower());
                await Clients.Client(ReceiverId).SendAsync("UserRemovedFromGroup", res);
            }
            await Clients.Caller.SendAsync("UserRemovedFromGroup", res);
        }

        public Response GetChats(int pageNumber = 1, int skipLimit = 30)
        {
            _logger.LogInformation("GetChats fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = _hubService.GetChatsService(userId,pageNumber,skipLimit);
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("ReceivedChats", res);
            return res;
        }

        public Response GetGroups(int pageNumber = 1, int skipLimit = 30)
        {
            _logger.LogInformation("GetGroups fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = _hubService.GetGroupsService(userId, pageNumber, skipLimit);
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("ReceivedGroups", res);
            return res;
        }

        public Response GetGroupInfo(string groupId)
        {
            _logger.LogInformation("Get Group info fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = _hubService.GetGroupInfoService(userId,groupId);
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("ReceivedGroupInfo", res);
            return res;
        }

        public Response GetGroupMembers(string groupId, int pageNumber = 1, int skipLimit = 30)
        {
            _logger.LogInformation("GetGroup members fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = _hubService.GetGroupMemberList(groupId, pageNumber, skipLimit).Result;
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("RecievedGroupMembers", res);
            return res;
        }

        public Response GetChatMessages(string chatId, int pageNumber,int skipLimit = 30)
        {
            _logger.LogInformation("GetChatMessages fxn called");
            var res = _hubService.GetChatMessagesService(chatId, pageNumber, skipLimit);
            //string ReceiverId = GetConnectionIdByUser(OtherMail);
            Clients.Caller.SendAsync("ReceivedChatMessages", res);
            //Clients.Client(ReceiverId).SendAsync("RecievedChatMessages", res);
            return res;
        }

        public async Task SeenMessage(string chatId,string messageId)
        {
            Console.WriteLine("Seen Message fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = await SeenMessageService(userId, chatId, messageId);
            //await Clients.Client(ReceiverId).SendAsync("GroupCreated", res);
            //await Clients.Caller.SendAsync("SeenMessage", res);
        }

        public async Task UpdateGroup(string groupId, string name, string description, string pathToPic)
        {
            Console.WriteLine("updateGroup fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = await _hubService.UpdateGroupService(userId, groupId, name, description, pathToPic);
            //await Clients.Client(ReceiverId).SendAsync("GroupCreated", res);
            await Clients.Caller.SendAsync("GroupUpdated", res);
        }

        public async Task DeleteGroup(string groupId)
        {
            Console.WriteLine("deleteGroup fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;

            var res = await _hubService.DeleteGroupService(userId, groupId);
            if (res.statusCode != 200)
            {
                await Clients.Caller.SendAsync("GroupDeleted", res);
                return;
            }
            var chatMaps = (List<ChatMappings>)res.data;
            foreach (var chatMap in chatMaps)
            {
                DbContext.ChatMappings.Remove(chatMap);
                string temp = GetConnectionIdByUser(chatMap.senderId.ToString().ToLower());
                await Clients.Client(temp).SendAsync("GroupDeleted", response);
            }
            await DbContext.SaveChangesAsync();
            return;
        }

        //-------------------------------- Helper functions --------------------------------------//
        public async Task ToggleUserOnlineState(User user, bool state)
        {
            if (user != null)
            {
                user.isOnline = state;
                user.lastActive = DateTime.Now;
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<int> refresh()
        {
            await Clients.All.SendAsync("refresh");
            return 0;
        }
        private void AddUserConnectionId(string userId)
        {
            _logger.LogInformation("User added to online dictionary ", userId);
            AddUserToList(userId.ToLower(), Context.ConnectionId);
        }

        public bool AddUserToList(string userToAdd, string connectionId)
        {
            lock (Users)
            {
                foreach (var user in Users)
                {
                    if (user.Key.ToLower() == userToAdd.ToLower())
                    {
                        return false;
                    }
                }

                Users.Add(userToAdd, connectionId);
                return true;
            }
        }

        public string GetUserByConnectionId(string connectionId)
        {
            lock (Users)
            {
                return Users.Where(x => x.Value == connectionId).Select(x => x.Key).FirstOrDefault()!;
            }
        }

        public string GetConnectionIdByUser(string userId)
        {
            lock (Users)
            {
                return Users.Where(x => x.Key == userId).Select(x => x.Value).FirstOrDefault()!;
            }
        }

        public void RemoveUserFromList(string userId)
        {
            lock (Users)
            {
                if (Users.ContainsKey(userId))
                {
                    Users.Remove(userId);
                }
            }
        }

        //get users email in dictionary
        public string[] GetOnlineUsers()
        {
            lock (Users)
            {
                return Users.OrderBy(x => x.Key).Select(x => x.Key).ToArray();
            }
        }


        //------------------------------------------------------------------------------------------------------------------//
        //----------------------------service functions-------------------------------------------------------------------//

        public async Task<Response> SeenMessageService(string userId, string chatId, string messageId)
        {
            Guid userGuid = new Guid(userId);
            Guid chatGuid = new Guid(chatId);
            Guid messageGuid = new Guid(messageId);

            Message? msg = DbContext.Messages.Find(messageGuid);
            ChatMappings? chatMap = DbContext.ChatMappings.Find(chatGuid);
            
            if(chatMap != null)
            {
                if(chatMap.isGroup)
                {
                    Guid groupId = chatMap.receiverId;
                    var exists = DbContext.GroupSeenMessageMappings.Where(s => (s.groupId == groupId && s.userId == userGuid && s.messageId == messageGuid)).FirstOrDefault();
                    if(exists == null)
                    {
                        GroupSeenMessageMappings seenMapping = new GroupSeenMessageMappings(messageGuid, groupId, userGuid);
                        await DbContext.GroupSeenMessageMappings.AddAsync(seenMapping);
                        await DbContext.SaveChangesAsync();
                    }
                    var listOfSeens = DbContext.GroupSeenMessageMappings.Where(s => (s.groupId == groupId && s.messageId==messageGuid)).ToList();
                    int countOfSeen = listOfSeens.Count();
                    Group? grp = DbContext.Groups.Find(groupId);
                    if (grp != null && msg != null && grp.noOfParticipants<=(countOfSeen+1))
                    {
                        msg.isSeen = true;
                        response = new Response(200, "Message Seen", msg, true);
                        foreach(var item in listOfSeens)
                        {
                            Guid receiverId = item.userId;
                            string temp = GetConnectionIdByUser(receiverId.ToString());
                            await Clients.Client(temp).SendAsync("SeenMessage", response);
                            DbContext.GroupSeenMessageMappings.Remove(item);
                        }
                        await DbContext.SaveChangesAsync();
                        Guid senderId = msg.senderId;
                        string tempId = GetConnectionIdByUser(senderId.ToString());
                        await Clients.Client(tempId).SendAsync("SeenMessage", response);
                    }
                }
                else
                {
                    if (msg != null)
                    {
                        msg.isSeen = true;
                    }
                    else
                    {
                        msg = new Message();
                    }
                    await DbContext.SaveChangesAsync();
                    response = new Response(200, "Message Seen", msg, true);
                    Guid receiverId = (chatMap.receiverId != new Guid(userId)) ? chatMap.receiverId : chatMap.senderId;
                    string temp = GetConnectionIdByUser(receiverId.ToString());
                    await Clients.Client(temp).SendAsync("SeenMessage", response);
                }
            }
            return response;
        }
    }
}
