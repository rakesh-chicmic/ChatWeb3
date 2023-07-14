using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Nethereum.JsonRpc.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ChatWeb3Frontend.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<APIResponse> GetYourselfAsync()
        {
            try
            {
                string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJiYTRkNjBkZC0wMmJhLTQxODAtYjQ3Yy1jNjBlMDFiN2FjMjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZmlyc3ROYW1lIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMHg2ZDc2MzY0ODZiMERkMmIxYTM0QmY5OEFkNDA3OEFjQzE3NDFmQzcyIiwiZXhwIjoxNjg5MzI3MzIyfQ.Tb7L2n_xjlc6EKyxXiMB7mfRCjY4LOvlm2ZFxo0L00wqVxUOPoBUM42zUjFS7f98a0QJYFEKkBvtKdBlui01LQ";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);             
                var response = await _httpClient.GetFromJsonAsync<APIResponse>($"api/v1/users/getYourself/");
                Console.WriteLine( response );
                return response;                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<APIResponse> UpdateAsync(UpdateUser update)
        {
            try
            {
                //string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJiYTRkNjBkZC0wMmJhLTQxODAtYjQ3Yy1jNjBlMDFiN2FjMjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZmlyc3ROYW1lIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMHg2ZDc2MzY0ODZiMERkMmIxYTM0QmY5OEFkNDA3OEFjQzE3NDFmQzcyIiwiZXhwIjoxNjg5MzI3MzIyfQ.Tb7L2n_xjlc6EKyxXiMB7mfRCjY4LOvlm2ZFxo0L00wqVxUOPoBUM42zUjFS7f98a0QJYFEKkBvtKdBlui01LQ";
                //var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"api/v1/users/registerUpdate");
                //requestMessage.Content = new StringContent(JsonSerializer.Serialize(update), Encoding.UTF8, "application/json");
                //requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //var result = await _httpClient.SendAsync(requestMessage);

                //var resultContent = await result.Content.ReadFromJsonAsync<APIResponse>();

                //Console.WriteLine( resultContent );
                //return resultContent!;
                string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJiYTRkNjBkZC0wMmJhLTQxODAtYjQ3Yy1jNjBlMDFiN2FjMjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZmlyc3ROYW1lIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMHg2ZDc2MzY0ODZiMERkMmIxYTM0QmY5OEFkNDA3OEFjQzE3NDFmQzcyIiwiZXhwIjoxNjg5MzI3MzIyfQ.Tb7L2n_xjlc6EKyxXiMB7mfRCjY4LOvlm2ZFxo0L00wqVxUOPoBUM42zUjFS7f98a0QJYFEKkBvtKdBlui01LQ";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsJsonAsync("api/v1/users/registerUpdate", update);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadFromJsonAsync<APIResponse>();
                    Console.WriteLine(res);
                    return res;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} Message -{message}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
