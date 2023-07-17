using ChatWeb3Frontend.Models;

namespace ChatWeb3Frontend.Services.Contracts
{
    public interface IAuthentication
    {
        Task<int> Login(Response inp);
        Task Logout();
        string GetToken();
    }
}
