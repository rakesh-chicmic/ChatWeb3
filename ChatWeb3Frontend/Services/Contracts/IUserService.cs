using ChatWeb3Frontend.Models;

namespace ChatWeb3Frontend.Services.Contracts
{
    public interface IUserService
    {
        Task<Response> UpdateAsync(UpdateUser update);
        Task<Response> GetAsync();
        Task<Response> ValidateUsernameAsync(string username);
    }
}
