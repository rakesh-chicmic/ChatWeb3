﻿using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Models.InputModels;
using ChatWeb3Frontend.Services.Contracts;
using Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ChatWeb3Frontend.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJiYTRkNjBkZC0wMmJhLTQxODAtYjQ3Yy1jNjBlMDFiN2FjMjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiUmFrZXNoIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMHg2ZDc2MzY0ODZiMERkMmIxYTM0QmY5OEFkNDA3OEFjQzE3NDFmQzcyIiwiZXhwIjoxNjkwMTczMDY1fQ.ePP_wYReF8Zgm6ZSgICf9kUxKfr8Ll92aq0m_yt-FI3yWLRv8x4zNPCTavE-n1HHwA8Lz2bto4M7l1aXPyqlwA";
       // string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiIyNTg3NjA4MS05MGRmLTRjNjAtYTZjYi0zM2UwN2MyNjE1MjkiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiIweDlCMEFGMjMzNDEzOUNmNUJiMkM2ZkIxNzgzQzc0NjU2MUYyNDFkNjgiLCJleHAiOjE2OTAzNjk5NDN9.91Lv3dE3Y-3gkLm2zTbQXwtO7sXpm_VmXGNBqiMHuDyhQBgU8HJf-mMFt5H6pqyKpomOSNeLxjb6DqsanuEXcw";
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        public async Task<Response> GetAsync()
        {
            try
            {           
                var apiResponse = await _httpClient.GetFromJsonAsync<Response>($"api/v1/users/getYourself/");
                return apiResponse;                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Response> UpdateAsync(UpdateUser update)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, "http://192.180.0.192:4545/api/v1/users/registerUpdate");
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(update), Encoding.UTF8, "application/json");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                var result = await _httpClient.SendAsync(requestMessage);

                if (!result.IsSuccessStatusCode)
                {
                    var message = await result.Content.ReadFromJsonAsync<Response>();
                    return new Response { statusCode = 0, message = message!.message };
                }
                var resultContent = await result.Content.ReadFromJsonAsync<Response>();
                Console.WriteLine(resultContent.ToString());
                return resultContent!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async  Task<Response> ValidateUsernameAsync(string username)
        {
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<Response>($"api/v1/users/validateUsername/?username={username}");
                return apiResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
