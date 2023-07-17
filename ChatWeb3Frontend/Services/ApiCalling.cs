using ChatWeb3Frontend.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace ChatWeb3Frontend.Services
{
    public class ApiCalling : IApiCalling
    {
        private readonly HttpClient _httpClient;
        public ApiCalling(HttpClient _httpClient)
        {
                this._httpClient = _httpClient;
        }
        public Response getYourself()
        {
            var loginResult =  _httpClient.GetFromJsonAsync($"{baseUrl}api/v1/login");
            return new Response { };
        }
    }
}
