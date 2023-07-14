using ChatWeb3Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ChatWeb3Frontend.Services.Contracts
{
    public interface IFileUploadService
    {
        Task<APIResponse> UploadProfileImageAsync(string file);
    }
}
