using ChatWeb3Frontend.Models;

namespace ChatWeb3Frontend.Services
{
    public interface IApiCalling
    {
        public Task<ResponseUser> getYourself();
        public Task<List<ResponseUser>> getUsers(string searchString);
    }
}
