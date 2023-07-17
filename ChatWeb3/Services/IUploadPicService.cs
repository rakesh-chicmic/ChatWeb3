using ChatWeb3Frontend.Models;

namespace ChatWeb3Frontend.Services
{
    public interface IUploadPicService
    {
        public Task<Response> FileUploadAsync(IFormFile? file, string id, int type);
        public Task<Response> ProfilePicUploadAsync(IFormFile? file, string id);
    }
}
