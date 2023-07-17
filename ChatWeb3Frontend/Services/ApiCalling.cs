using ChatWeb3Frontend.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChatWeb3Frontend.Services
{
    public class ApiCalling : IApiCalling
    {
        private readonly HttpClient _httpClient;
        public ApiCalling(HttpClient _httpClient)
        {
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
    }
}
