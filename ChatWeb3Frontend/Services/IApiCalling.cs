using ChatWeb3Frontend.Models;
using Microsoft.AspNetCore.Components;

namespace ChatWeb3Frontend.Services
{
    public interface IApiCalling
    {
        public Task<ResponseUser> getYourself();
        public Task<List<ResponseUser>> getUsers(string searchString);
        public Task<Response> UploadFileAsync(ElementReference elementReference, int type);
    }
}
