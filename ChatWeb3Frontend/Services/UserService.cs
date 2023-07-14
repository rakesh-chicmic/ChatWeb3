using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;
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
        string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJhOTk3NDE5OC0zNzVmLTQ5YTAtYmQxMy1kYTVlYTQxYzQyM2EiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiUmFrZXNoIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMHhkQjc2RjgwM0FCOWYzNTBGOTI4QUQ1MzM1ZkI4YzA5RThhMzYyMjVhIiwiZXhwIjoxNjg5NDAyMTAzfQ.M_8yvrcfhuLplhMEKFRh1FyxODm2R4TSX9SN3NwPW8D6przcrEhtl8XQa_Q9HknCRCyqlkB0ZmaX_m4QeDTfdw";
        
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        public async Task<APIResponse> GetAsync()
        {
            try
            {           
                var apiResponse = await _httpClient.GetFromJsonAsync<APIResponse>($"api/v1/users/getYourself/");
                return apiResponse;                
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
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, "http://192.180.0.192:4545/api/v1/users/registerUpdate");
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(update), Encoding.UTF8, "application/json");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                var result = await _httpClient.SendAsync(requestMessage);

                if (!result.IsSuccessStatusCode)
                {
                    var message = await result.Content.ReadFromJsonAsync<APIResponse>();
                    return new APIResponse { statusCode = 0, message = message!.message };
                }
                var resultContent = await result.Content.ReadFromJsonAsync<APIResponse>();
                Console.WriteLine(resultContent.ToString());
                return resultContent!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
