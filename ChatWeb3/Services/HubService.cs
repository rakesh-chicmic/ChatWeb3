using ChatWeb3.Controllers;
using ChatWeb3.Data;
using ChatWeb3.Models;

namespace ChatWeb3.Services
{
    public class HubService : IHubService
    {
        Response response = new Response();         //response model
        private readonly ChatAppDbContext DbContext;    //orm 
        private readonly IConfiguration _configuration;     //config settings

        //-------------------------- Constructor --------------------------------------------//
        public HubService(IConfiguration configuration, ChatAppDbContext dbContext, ILogger<UserController> logger)
        {
            this._configuration = configuration;
            DbContext = dbContext;
        }

        //function to create a new chat mapping b/w two users
        public async Task<Response> AddChat(string firstId, string secondId)
        {
            Guid senderId = new Guid(firstId);
            Guid receiverId = new Guid(secondId);
            var chatMapdb = DbContext.ChatMappings.ToList();
            var user1 = DbContext.Users.Find(senderId);
            var user2 = DbContext.Users.Find(receiverId);
            if (user2 == null)
            {
                response = new Response(400, "User you are trying to connect does not exist", "", true);
                return response;
            }
            var chats = chatMapdb.Where(c => c.senderId == senderId && c.receiverId == receiverId).FirstOrDefault();
            if (chats == null)
            {
                chats = chatMapdb.Where(c => c.senderId == receiverId && c.receiverId == senderId).FirstOrDefault();
            }

            if (chats == null)
            {
                ChatMappings chatMap = new ChatMappings(senderId, receiverId, false);
                await DbContext.ChatMappings.AddAsync(chatMap);
                await DbContext.SaveChangesAsync();
                chats = chatMap;
                /* response.Data = output;*/
            }

            OutputChatMappings output = new OutputChatMappings(user2, chats, 0);

            response = new Response(200, "Chat created/ fetched", output, true);
            return response;
        }

        //function to add a new message into db
        public OutputMessage AddMessage(string sender, string chat, string content, int type, string path)
        {
            Guid senderId = new Guid(sender);
            Guid chatId = new Guid(chat);
            Message message = new Message(senderId, chatId, content, type, path);
            OutputMessage result = new OutputMessage(message);
            DbContext.Messages.Add(message);
            DbContext.SaveChanges();
            return result;
        }

        //function to create a group 
        public async Task<Response> AddGroup(string userId, string name, string description, string pathToPic)
        {
            Guid userGuid = new Guid(userId);
            Group group = new Group(name, description, userGuid, pathToPic, 1);
            await DbContext.Groups.AddAsync(group);

            ChatMappings map = new ChatMappings(userGuid, group.id, true);
            await DbContext.ChatMappings.AddAsync(map);
            ResponseGroup output = new ResponseGroup(group);
            response = new Response(200, "Group created", output, true);
            await DbContext.SaveChangesAsync();
            return response;
        }

        //function to update a group can be performed only by a admin
        public async Task<Response> UpdateGroupService(string userId, string groupId, string name, string description, string pathToPic)
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

        //function to admin a user from a group can be performed only by a admin
        public async Task<Response> AddUserToGroup(string adminId, string groupId, string userId)
        {
            Guid userGuid = new Guid(userId);
            Guid groupGuid = new Guid(groupId);
            Guid adminGuid = new Guid(adminId);

            User? user = DbContext.Users.Find(userGuid);
            if (user == null)
            {
                response = new Response(400, "User doesn't exist", "", false);
                return response;
            }

            Group? grp = await DbContext.Groups.FindAsync(groupGuid);
            if (grp != null)
            {
                if (grp.adminId != adminGuid)
                {
                    response = new Response(400, "Not authorized", "", false);
                    return response;
                }
                //check if already exist
                ChatMappings? map = DbContext.ChatMappings.Where(s => s.senderId == userGuid && s.receiverId == groupGuid).FirstOrDefault();
                if (map == null)
                {
                    map = new ChatMappings(userGuid, grp.id, true);
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
        
        //function to remove a user from a group can be performed only by a admin
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
                ChatMappings? map = DbContext.ChatMappings.Where(s => s.senderId == userGuid && s.receiverId == groupGuid).FirstOrDefault();
                if (map != null)
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
            chatMaps = chatMaps.Where(s => ((s.senderId == userId) || (s.receiverId == userId)) && s.isGroup == false).ToList();

            //chatMaps.Remove(chatMaps.Where(s => s.senderId == s.receiverId).FirstOrDefault());
            List<OutputChatMappings> list = new List<OutputChatMappings>();
            OutputChatMappings output = new OutputChatMappings() { };
            foreach (var cm in chatMaps)
            {
                var user1 = DbContext.Users.Find(cm.senderId);
                var user2 = DbContext.Users.Find(cm.receiverId);
                if (user1 != null && user1.id != userId)
                {
                    int countOfUnseen = DbContext.Messages.Where(s => (s.chatId == cm.id && s.senderId == user1.id && s.isSeen == false)).Count();
                    output = new OutputChatMappings(user1, cm, countOfUnseen);
                }
                else
                {
                    int countOfUnseen = DbContext.Messages.Where(s => (s.chatId == cm.id && s.senderId == user2!.id && s.isSeen == false)).Count();
                    output = new OutputChatMappings(user2!, cm, countOfUnseen);
                }
                list.Add(output);
            }

            int totalCount = list.Count;
            Console.WriteLine(totalCount);
            list = list.OrderByDescending(x => x.dateTime).ToList();
            list = list.Skip((pageNumber - 1) * skipLimit).Take(skipLimit).ToList();
            PaginationCountList<OutputChatMappings> result = new PaginationCountList<OutputChatMappings>(totalCount, list);
            response = new Response(200, "Chat list fetched", result, true);
            return response;
        }

        //function invoked to get all group chat mappings created for a particular user
        public Response GetGroupsService(string id, int pageNumber, int skipLimit)
        {
            var chatMaps = DbContext.ChatMappings.ToList();
            Guid userId = new Guid(id);
            chatMaps = chatMaps.Where(s => (s.senderId == userId) && s.isGroup == true).ToList();

            List<OutputGroups> list = new List<OutputGroups>();
            //OutputChatMappings output = new OutputChatMappings() { };
            foreach (var cm in chatMaps)
            {
                Group gr = DbContext.Groups.Find(cm.receiverId)!;
                var grpChatMappings = DbContext.ChatMappings.Where(s => s.receiverId == gr.id).Select(s => s.id).ToList();
                //gets total unseen by all and the ones seen by user among those then subtratcts later from former to get final unseen count
                int unseenGroupMessages = DbContext.Messages.Where(s => (grpChatMappings.Contains(s.chatId) && s.isSeen == false && s.senderId != userId)).Count();
                int userSeenAmongUnseen = DbContext.GroupSeenMessageMappings.Where(s => s.groupId == gr.id && s.userId == userId).Count();
                int answerUnseen = unseenGroupMessages - userSeenAmongUnseen;

                answerUnseen = (answerUnseen > 0) ? answerUnseen : 0;

                OutputGroups output = new OutputGroups(gr, cm, unseenGroupMessages);
                list.Add(output);
            }
            int totalCount = list.Count;
            Console.WriteLine(totalCount);
            list = list.OrderByDescending(x => x.datetime).ToList();
            list = list.Skip((pageNumber - 1) * skipLimit).Take(skipLimit).ToList();
            PaginationCountList<OutputGroups> result = new PaginationCountList<OutputGroups>(totalCount, list);
            response = new Response(200, "Groups list fetched", result, true);
            return response;
        }

        //function invoked to get all info for a particular group
        public Response GetGroupInfoService(string id, string groupId)
        {
            Guid userId = new Guid(id);
            Guid groupGuid = new Guid(groupId);
            var chatMap = DbContext.ChatMappings.Where(s => s.isGroup == true && s.senderId == userId && s.receiverId == groupGuid).FirstOrDefault();
            if (chatMap == null)
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
            if (grp == null)
            {
                return new Response(200, "This Group doesn't exists", "", true);
            }
            List<Guid> userIds = DbContext.ChatMappings.Where(s => s.isGroup == true && s.receiverId == groupGuid).Select(s => s.senderId).ToList();
            List<User> listUser = DbContext.Users.Where(s => userIds.Contains(s.id)).ToList();
            List<ResponseUser> list = new List<ResponseUser>();

            foreach (User user in listUser)
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
            messages = messages.Where(m => (m.chatId == chatGuid)).ToList();

            int totalCount = messages.Count;
            messages = messages.Skip((pageNumber - 1) * skipLimit).Take(skipLimit).ToList();
            messages = messages.OrderByDescending(m => m.createdAt).Select(m => m).ToList();

            List<OutputMessage> res = new List<OutputMessage>();

            foreach (var msg in messages)
            {
                OutputMessage output = new OutputMessage(msg);
                res.Add(output);
            }
            //res.Reverse();
            PaginationCountList<OutputMessage> result = new PaginationCountList<OutputMessage>(totalCount, res);
            response = new Response(200, "Chat messages fetched", result, true);
            return response;
        }

        // function invoked to delete a group 
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
                response = new Response(200, "Group Deleted", chatMaps, true);
            }
            await DbContext.SaveChangesAsync();
            return response;
        }

    }
}
