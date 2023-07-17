using ChatWeb3Frontend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWeb3Frontend.Services
{
    public interface IHubService
    {
        public Task<Response> AddChat(string firstId, string secondId);
        public OutputMessage AddMessage(string sender, string chat, string content, int type, string path);
        public Task<Response> AddGroup(string userId, string name, string description, string pathToPic);
        public Task<Response> UpdateGroupService(string userId, string groupId, string name, string description, string pathToPic);
        public Task<Response> AddUserToGroup(string adminId, string groupId, string userId);
        public Task<Response> RemoveUserFromGroup(string adminId, string groupId, string userId);
        public Response GetChatsService(string id, int pageNumber, int skipLimit);
        public Response GetGroupsService(string id, int pageNumber, int skipLimit);
        public Response GetGroupInfoService(string id, string groupId);
        public Task<Response> GetGroupMemberList(string groupId, int pageNumber, int skipLimit);
        public Response GetChatMessagesService(string chatId, int pageNumber, int skipLimit);
        public Task<Response> DeleteGroupService(string userId, string groupId);
    }
}
