using ChatWeb3.Models;

namespace ChatWeb3.Services
{
    public interface IUploadPicService
    {
        public Task<Response> FileUploadAsync(IFormFile? file, string id, int type);
        public Task<Response> ProfilePicUploadAsync(IFormFile? file, string id);
    }
}
