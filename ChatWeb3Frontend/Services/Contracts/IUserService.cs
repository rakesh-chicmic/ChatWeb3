using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Models.InputModels;

namespace ChatWeb3Frontend.Services.Contracts
{
    public interface IUserService
    {
        Task<Response> UpdateAsync(UpdateUser update);
        Task<Response> GetAsync();
        Task<Response> ValidateUsernameAsync(string username);
    }
}
