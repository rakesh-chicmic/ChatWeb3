using ChatWeb3Frontend.Models;
using Microsoft.AspNetCore.Components;
using System.IO.Pipelines;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Tewr.Blazor.FileReader;

namespace ChatWeb3Frontend.Services
{
    public class ApiCalling : IApiCalling
    {
        IFileReaderService fileReader { get; set; }
        private readonly HttpClient _httpClient;
        public ApiCalling(HttpClient _httpClient,IFileReaderService fileReader)
        {
            this.fileReader = fileReader;
            this._httpClient = _httpClient;
        }
        public async Task<ResponseUser> getYourself()
        {
            var response = await _httpClient.GetFromJsonAsync<Response>($"https://localhost:7218/api/v1/users/getYourself");
            
            return JsonSerializer.Deserialize<ResponseUser>(JsonSerializer.Serialize(response!.data))!;
        }

        public async Task<List<ResponseUser>> getUsers(string searchString)
        {
            var response = await _httpClient.GetFromJsonAsync<Response>($"https://localhost:7218/api/v1/users/get?searchString={searchString}&OrderBy=username&SortOrder=1&RecordsPerPage=20&PageNumber=0");

            return JsonSerializer.Deserialize<List<ResponseUser>>(JsonSerializer.Serialize(response!.data))!;
        }

        public async Task<Response> UploadFileAsync(ElementReference elementReference,int type)
        {
            var file = (await fileReader.CreateReference(elementReference).EnumerateFilesAsync()).FirstOrDefault();
            var fileInfo = await file.ReadFileInfoAsync();
            string fileName = fileInfo.Name;
            Stream fileStream = null;
            using (var memoryStream = await file.CreateMemoryStreamAsync((int)fileInfo.Size))
            {
                fileStream = new MemoryStream(memoryStream.ToArray());
            }
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Add(new StreamContent(fileStream, (int)fileStream.Length), "file", fileName);
            var response = await _httpClient.PostAsync($"https://localhost:7218/api/v1/uploadFile?type={type}", content);
            var apiResponse = await response.Content.ReadFromJsonAsync<Response>();
            return apiResponse!;
        }
    }
}
