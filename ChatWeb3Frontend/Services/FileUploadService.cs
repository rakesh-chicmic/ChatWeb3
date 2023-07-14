using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;

namespace ChatWeb3Frontend.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient _httpClient;

        public FileUploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<APIResponse> UploadProfileImageAsync(string file)
        {
            throw new NotImplementedException();
        }

        //public async Task<APIResponse> UploadProfileImageAsync(string file)
        //{

        //    try
        //    {
        //        // Send the form data to the API endpoint
        //       var response = await _httpClient.PostAsync("/api/v1/uploadProfilePic",file);

        //        // Handle the response as needed
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return await response.Content.ReadFromJsonAsync<APIResponse>();
        //        }
        //        else
        //        {
        //            var message = await response.Content.ReadAsStringAsync();
        //            throw new Exception($"Http status:{response.StatusCode} Message -{message}");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
