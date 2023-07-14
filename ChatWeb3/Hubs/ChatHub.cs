using ChatWeb3.Data;
using ChatWeb3.Models;
using ChatWeb3.Models.OutputModels;
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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        // to keep track of online users dict key-value pair
        // key - userId     value - connectionId
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();
        Response response = new Response();
        private readonly ChatAppDbContext DbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ChatAppDbContext dbContext, IConfiguration configuration,ILogger<ChatHub> logger)
        {
            _configuration = configuration;
            DbContext = dbContext;
            _logger = logger;
        }

        public async override Task<Task> OnConnectedAsync()
        {
            try
            {
                _logger.LogInformation("new user connected");
                string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;               //get user id
                await Clients.Caller.SendAsync("UserConnected", Context.ConnectionId);              // reply back to connection to confirm his connection to socket
                User user = DbContext.Users.Find(new Guid(userId))!;
                //await Clients.Caller.SendAsync("User", user);
                if(user != null)
                {
                    user.isOnline = true;
                    user.lastActive = DateTime.Now;
                    await DbContext.SaveChangesAsync();
                }
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
            User? user = await DbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.isOnline = false;
                user.lastActive = DateTime.Now;
                await DbContext.SaveChangesAsync();
            }
            //await OnlineUsers();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<int> refresh()
        {
            await Clients.All.SendAsync("refresh");
            return 0;
        }

        //public async Task AddUserConnectionId(string userId)
        public void AddUserConnectionId(string userId)
        {
            _logger.LogInformation("User added to online dictionary ", userId);
            AddUserToList(userId.ToLower(), Context.ConnectionId);
            //await OnlineUsers();
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
                    await SendMessageHelper(message, member.id);
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
                await SendMessageHelper(message, receiverId);
            }
            return;
        }

        private async Task SendMessageHelper(Message message,Guid receiverId)
        {
            string receiverConnectionId = GetConnectionIdByUser(receiverId.ToString().ToLower());
            var fileName = message.pathToFileAttachment!.Split("/").Last();

            OutputMessage sendMsg = new OutputMessage(message);

            //await Clients.Caller.SendAsync("ReceivedMessage", sendMsg);
            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivedMessage", sendMsg);
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
            var res = await AddChat(userId,ConnectToId);
            await Clients.Clients(ReceiverId).SendAsync("ChatCreated", res);
            await Clients.Caller.SendAsync("ChatCreated", res);
        }
        public async Task CreateGroup(string name, string description,string pathToPic)
        {
            Console.WriteLine("createGroup fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            
            var res = await AddGroup(userId, name, description, pathToPic);
            //await Clients.Client(ReceiverId).SendAsync("GroupCreated", res);
            await Clients.Caller.SendAsync("GroupCreated", res);
        }

        public async Task AddMemberToGroup(string groupId, string userToAdd)
        {
            Console.WriteLine("add user to grp fxn called");
            string adminId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            //Guid adminGuid = Guid.Parse(adminId);
            var res = await AddUserToGroup(adminId, groupId, userToAdd);
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
            var res = await RemoveUserFromGroup(adminId, groupId, userToRemove);
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
            var res = GetChatsService(userId,pageNumber,skipLimit);
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("RecievedChats", res);
            return res;
        }

        public Response GetGroups(int pageNumber = 1, int skipLimit = 30)
        {
            _logger.LogInformation("GetGroups fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = GetGroupsService(userId, pageNumber, skipLimit);
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("RecievedGroups", res);
            return res;
        }

        public Response GetGroupInfo(string groupId)
        {
            _logger.LogInformation("Get Group info fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = GetGroupInfoService(userId,groupId);
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("RecievedGroupInfo", res);
            return res;
        }

        public Response GetGroupMembers(string groupId, int pageNumber = 1, int skipLimit = 30)
        {
            _logger.LogInformation("GetGroup members fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;
            var res = GetGroupMemberList(groupId, pageNumber, skipLimit).Result;
            string Id = GetConnectionIdByUser(userId);
            Clients.Client(Id).SendAsync("RecievedGroupMembers", res);
            return res;
        }

        public Response GetChatMessages(string chatId, int pageNumber,int skipLimit = 30)
        {
            _logger.LogInformation("GetChatMessages fxn called");
            var res = GetChatMessagesService(chatId, pageNumber, skipLimit);
            //string ReceiverId = GetConnectionIdByUser(OtherMail);
            Clients.Caller.SendAsync("RecievedChatMessages", res);
            //Clients.Client(ReceiverId).SendAsync("RecievedChatMessages", res);
            return res;
        }

        public async Task UpdateGroup(string groupId,string name, string description, string pathToPic)
        {
            Console.WriteLine("updateGroup fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;

            var res = await UpdateGroupService(userId, groupId,name, description, pathToPic);
            //await Clients.Client(ReceiverId).SendAsync("GroupCreated", res);
            await Clients.Caller.SendAsync("GroupUpdated", res);
        }

        public async Task DeleteGroup(string groupId)
        {
            Console.WriteLine("deleteGroup fxn called");
            string userId = Context.User!.FindFirstValue(ClaimTypes.PrimarySid)!;

            var res = await DeleteGroupService(userId, groupId);
            if (res.statusCode != 200)
            {
                await Clients.Caller.SendAsync("GroupDeleted", res);
            }

            return;
            //await Clients.Caller.SendAsync("GroupDeleted", res);
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

        //create new message and add in database
        public OutputMessage AddMessage(string sender, string chat, string content, int type, string path)
        {
            Guid senderId = new Guid(sender);
            Guid chatId = new Guid(chat);
            Message message = new Message(senderId,chatId,content,type,path);

            OutputMessage result = new OutputMessage(message);
            DbContext.Messages.Add(message);
            DbContext.SaveChanges();
            return result;
        }

        // create a new chat mapping, if already exist then send it
        public async Task<Response> AddChat(string firstId, string secondId)
        {
            Guid senderId = new Guid(firstId);
            Guid receiverId = new Guid(secondId);
            var chatMapdb = DbContext.ChatMappings.ToList();
            var user1 = DbContext.Users.Find(senderId);
            var user2 = DbContext.Users.Find(receiverId);
            if (user2 == null)
            {
                response = new Response(400,"User you are trying to connect does not exist","",true);
                return response;
            }
            var chats = chatMapdb.Where(c => c.senderId == senderId && c.receiverId == receiverId).FirstOrDefault();
            if (chats == null)
            {
                chats = chatMapdb.Where(c => c.senderId == receiverId && c.receiverId == senderId).FirstOrDefault();
            }

            if (chats == null)
            {
                ChatMappings chatMap = new ChatMappings(senderId,receiverId,false);
                await DbContext.ChatMappings.AddAsync(chatMap);
                await DbContext.SaveChangesAsync();
                chats = chatMap;
                /* response.Data = output;*/
            }

            OutputChatMappings output = new OutputChatMappings( user2, chats);

            response = new Response(200,"Chat created/ fetched",output,true);
            return response;
        }

        public async Task<Response> AddGroup(string userId, string name, string description,string pathToPic)
        {
            Guid userGuid = new Guid(userId);
            Group group = new Group(name, description, userGuid, pathToPic, 1);
            await DbContext.Groups.AddAsync(group);

            ChatMappings map = new ChatMappings(userGuid,group.id,true);
            await DbContext.ChatMappings.AddAsync(map);
            ResponseGroup output = new ResponseGroup(group);
            response = new Response(200, "Group created", output, true);
            await DbContext.SaveChangesAsync();
            return response;
        }

        public async Task<Response> UpdateGroupService(string userId, string groupId,string name, string description, string pathToPic)
        {
            Guid userGuid = new Guid(userId);
            Guid groupGuid = new Guid(groupId);

            Group? grp = await DbContext.Groups.FindAsync(groupGuid);
            ResponseGroup output = new ResponseGroup();
            if (grp != null)
            {
                if (grp.adminId != userGuid)
                {
                    response = new Response(400, "Not authorized", "", true);
                    return response;
                }
                grp.name = name;
                grp.description = description;
                grp.pathToProfilePic = pathToPic;
                output = new ResponseGroup(grp);
            }

            await DbContext.SaveChangesAsync();
            response = new Response(200, "Group updated", output, true);
            return response;
        }

        public async Task<Response> DeleteGroupService(string userId, string groupId)
        {
            Guid userGuid = new Guid(userId);
            Guid groupGuid = new Guid(groupId);
            Group? grp = await DbContext.Groups.FindAsync(groupGuid);
            if (grp != null)
            {
                if (grp.adminId != userGuid)
                {
                    response = new Response(400, "Not authorized", "", true);
                    return response;
                }
                grp.isDeleted = true;
                
                var chatMaps = DbContext.ChatMappings.Where(s => s.isGroup == true && s.receiverId == groupGuid).ToList();
                response = new Response(200, "Group Deleted", groupId, true);
                foreach ( var chatMap in chatMaps )
                {
                    DbContext.ChatMappings.Remove(chatMap);
                    string temp = GetConnectionIdByUser(chatMap.senderId.ToString().ToLower());
                    await Clients.Client(temp).SendAsync("GroupDeleted", response);
                }
            }
            await DbContext.SaveChangesAsync();
            return response;
        }

        public async Task<Response> AddUserToGroup(string adminId, string groupId, string userId)
        {
            Guid userGuid = new Guid(userId);
            Guid groupGuid = new Guid(groupId);
            Guid adminGuid = new Guid(adminId);
            
            User? user = DbContext.Users.Find(userGuid);
            if(user == null)
            {
                response = new Response(400, "User doesn't exist", "", false);
                return response;
            }

            Group? grp = await DbContext.Groups.FindAsync(groupGuid);
            if(grp != null)
            {
                if(grp.adminId != adminGuid)
                {
                    response = new Response(400, "Not authorized", "", false);
                    return response;
                }
                //check if already exist
                ChatMappings? map = DbContext.ChatMappings.Where(s => s.senderId == userGuid && s.receiverId == groupGuid).FirstOrDefault();
                if(map == null)
                {
                    map = new ChatMappings(userGuid,grp.id,true);
                    await DbContext.ChatMappings.AddAsync(map);
                    grp.noOfParticipants++;
                }
                else
                {
                    response = new Response(400, "User already exists in group", "", true);
                    return response;
                }
            }

            ResponseGroup output = new ResponseGroup(grp!);
            response = new Response(200, "User added", output, true);
            await DbContext.SaveChangesAsync();
            return response;
        }

        public async Task<Response> RemoveUserFromGroup(string adminId, string groupId, string userId)
        {
            Guid userGuid = new Guid(userId);
            Guid groupGuid = new Guid(groupId);
            Guid adminGuid = new Guid(adminId);

            Group? grp = await DbContext.Groups.FindAsync(groupGuid);
            if (grp != null)
            {
                if (grp.adminId != adminGuid)
                {
                    response = new Response(400, "Not authorized", "", true);
                    return response;
                }
                ChatMappings? map =  DbContext.ChatMappings.Where(s => s.senderId == userGuid && s.receiverId == groupGuid).FirstOrDefault();
                if(map != null)
                {
                    grp.noOfParticipants--;
                    DbContext.ChatMappings.Remove(map);
                }
                else
                {
                    response = new Response(400, "User doesn't exists in group", "", true);
                    return response;
                }
            }

            ResponseGroup output = new ResponseGroup(grp!);
            response = new Response(200, "User removed", output, true);
            await DbContext.SaveChangesAsync();
            return response;
        }

        //function invoked to get all chat mappings created for a particular user
        public Response GetChatsService(string id, int pageNumber, int skipLimit)
        { 
            var chatMaps = DbContext.ChatMappings.ToList();
            Guid userId = new Guid(id);
            chatMaps = chatMaps.Where(s => ((s.senderId == userId) || (s.receiverId == userId)) && s.isGroup==false).ToList();

            //chatMaps.Remove(chatMaps.Where(s => s.senderId == s.receiverId).FirstOrDefault());
            List<OutputChatMappings> list = new List<OutputChatMappings>();
            OutputChatMappings output = new OutputChatMappings() { };
            foreach (var cm in chatMaps)
            {
                var user1 = DbContext.Users.Find(cm.senderId);
                var user2 = DbContext.Users.Find(cm.receiverId);
                if(user1 != null && user1.id != userId)
                {
                    output = new OutputChatMappings(user1, cm);
                }
                else
                {
                    output = new OutputChatMappings(user2!, cm);
                }
                list.Add(output);
            }

            int totalCount = list.Count;
            Console.WriteLine(totalCount);
            list = list.OrderByDescending(x => x.dateTime).ToList();
            list = list.Skip((pageNumber - 1) * skipLimit).Take(skipLimit).ToList();
            PaginationCountList<OutputChatMappings> result  = new PaginationCountList<OutputChatMappings>(totalCount,list);
            response = new Response(200, "Chat list fetched", result, true);
            return response;
        }

        public Response GetGroupsService(string id, int pageNumber, int skipLimit)
        {
            var chatMaps = DbContext.ChatMappings.ToList();
            Guid userId = new Guid(id);
            chatMaps = chatMaps.Where(s => (s.senderId == userId)  && s.isGroup == true).ToList();

            List<OutputGroups> list = new List<OutputGroups>();
            //OutputChatMappings output = new OutputChatMappings() { };
            foreach (var cm in chatMaps)
            {
                Group gr = DbContext.Groups.Find(cm.receiverId)!;
                OutputGroups output = new OutputGroups(gr,cm.datetime);
                list .Add(output);
            }
            int totalCount = list.Count;
            Console.WriteLine(totalCount);
            list = list.OrderByDescending(x => x.datetime).ToList();
            list = list.Skip((pageNumber - 1) * skipLimit).Take(skipLimit).ToList();
            PaginationCountList<OutputGroups> result = new PaginationCountList<OutputGroups>(totalCount, list);
            response = new Response(200,"Groups list fetched",result,true);
            return response;
        }

        public Response GetGroupInfoService(string id, string groupId)
        {
            Guid userId = new Guid(id);
            Guid groupGuid = new Guid(groupId);
            var chatMap = DbContext.ChatMappings.Where(s=>s.isGroup  == true && s.senderId==userId && s.receiverId == groupGuid).FirstOrDefault();
            if(chatMap == null)
            {
                response = new Response(403, "Not authorized", new Group(), true);
                return response;
            }
            Group? grp = DbContext.Groups.Find(groupGuid);
            ResponseGroup result = new ResponseGroup(grp!);
            response = new Response(200, "Group info fetched", result, true);
            return response;
        }

        public async Task<Response> GetGroupMemberList(string groupId, int pageNumber, int skipLimit)
        {
            Guid groupGuid = new Guid(groupId);
            Group? grp = await DbContext.Groups.FindAsync(groupGuid);
            if(grp == null)
            {
                return new Response(200, "This Group doesn't exists", "", true);
            }
            List<Guid> userIds =  DbContext.ChatMappings.Where(s => s.isGroup==true && s.receiverId== groupGuid).Select(s=>s.senderId).ToList();
            List<User> listUser = DbContext.Users.Where(s=>userIds.Contains(s.id)).ToList();
            List<ResponseUser> list = new List<ResponseUser>();

            foreach(User user in listUser)
            {
                ResponseUser responseUser = new ResponseUser(user);
                list.Add(responseUser);
            }
            int totalCount = list.Count;
            Console.WriteLine(totalCount);
            list = list.OrderByDescending(x => x.username).ToList();
            list = list.Skip((pageNumber - 1) * skipLimit).Take(skipLimit).ToList();
            PaginationCountList<ResponseUser> result = new PaginationCountList<ResponseUser>(totalCount, list);
            response = new Response(200, "Members list", result, true);
            return response;
        }

        // function invoked to get previous chat between two users
        public Response GetChatMessagesService(string chatId, int pageNumber, int skipLimit)
        {
            var messages = DbContext.Messages.ToList();
            Guid chatGuid = new Guid(chatId);
            messages = messages.Where(m => (m.chatId == chatGuid )).ToList();

            messages = messages.OrderByDescending(m => m.createdAt).Select(m => m).ToList();
            int totalCount = messages.Count;
            messages = messages.Skip((pageNumber - 1) * skipLimit).Take(skipLimit).ToList();

            List<OutputMessage> res = new List<OutputMessage>();

            foreach (var msg in messages)
            {
                OutputMessage output = new OutputMessage(msg);
                res.Add(output);
            }
            //res.Reverse();
            PaginationCountList<OutputMessage> result = new PaginationCountList<OutputMessage>(totalCount,res);
            response = new Response(200,"Chat messages fetched",result, true);
            return response;
        }
        //-----------------------------------------------------------------------------------------------------------------//
}
}
