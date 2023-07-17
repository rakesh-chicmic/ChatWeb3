using ChatWeb3Frontend.Models;
using Microsoft.AspNetCore.Components;

namespace ChatWeb3Frontend.Services.Contracts
{
    public interface IFileUploadService
    {
        Task<Response> UploadFileAsync(ElementReference elementReference);
    }
}
