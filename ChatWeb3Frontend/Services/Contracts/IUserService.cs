using ChatWeb3Frontend.Models;

namespace ChatWeb3Frontend.Services.Contracts
{
    public interface IUserService
    {
        Task<APIResponse> UpdateAsync(UpdateUser update);
        Task<APIResponse> GetAsync();
        Task<APIResponse> ValidateUsernameAsync(string username);
    }
}
