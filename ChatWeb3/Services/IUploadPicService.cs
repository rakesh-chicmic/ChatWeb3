using ChatWeb3.Models;

namespace ChatWeb3.Services
{
    public interface IUploadPicService
    {
        public Task<object> FileUploadAsync(IFormFile file, string id, int type);
        public Task<object> ProfilePicUploadAsync(IFormFile file, string id);
    }
}
