using ChatWeb3.Models;

namespace ChatWeb3.Services
{
    public interface IUserService
    {
        public Response GetYourself(string id);
        public Response GetUsers(string userId,string? searchString = null, string? accountAddress = null, string? id = null, string OrderBy = "username", int SortOrder = 1, int RecordsPerPage = 20, int PageNumber = 0);
        public Task<Response> UpdateUser(string id, UpdateUser update);
        public Task<Response> DeleteUser(string id);
    }
}
