using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Nethereum.JsonRpc.Client;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Tewr.Blazor.FileReader;

namespace ChatWeb3Frontend.Services
{
    public class FileUploadService : IFileUploadService
    {
        IFileReaderService fileReader { get; set; }
        private readonly HttpClient _httpClient;
        string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJiYTRkNjBkZC0wMmJhLTQxODAtYjQ3Yy1jNjBlMDFiN2FjMjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiUmFrZXNoIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMHg2ZDc2MzY0ODZiMERkMmIxYTM0QmY5OEFkNDA3OEFjQzE3NDFmQzcyIiwiZXhwIjoxNjkwMTczMDY1fQ.ePP_wYReF8Zgm6ZSgICf9kUxKfr8Ll92aq0m_yt-FI3yWLRv8x4zNPCTavE-n1HHwA8Lz2bto4M7l1aXPyqlwA";

        public FileUploadService(HttpClient httpClient, IFileReaderService fileReader)
        {
            this.fileReader = fileReader;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
        public async Task<Response> UploadFileAsync(ElementReference elementReference)
        {
            var file = (await fileReader.CreateReference(elementReference).EnumerateFilesAsync()).FirstOrDefault();
            var fileInfo = await file.ReadFileInfoAsync();
            string fileName = fileInfo.Name;
            string size = $"{fileInfo.Size}b";
            string type = fileInfo.Type;
            Stream fileStream = null;
            using (var memoryStream = await file.CreateMemoryStreamAsync((int)fileInfo.Size))
            {
                fileStream = new MemoryStream(memoryStream.ToArray());
            }
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Add(new StreamContent(fileStream, (int)fileStream.Length), "file", fileName);
            var response = await _httpClient.PostAsync("api/v1/uploadProfilePic", content);
            var apiResponse = await response.Content.ReadFromJsonAsync<Response>();
            return apiResponse;
        }

    }
}
