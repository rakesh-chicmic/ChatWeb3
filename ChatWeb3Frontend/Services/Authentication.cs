using Blazored.LocalStorage;
using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Auth;
using ChatWeb3Frontend.Services.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChatWeb3Frontend.Services
{
    public class Authentication : IAuthentication
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _configuration;

        private readonly string baseUrl;
        private string? token { get; set; } = null;

        public Authentication(HttpClient httpClient, AuthenticationStateProvider authStateProvider, IConfiguration configuration,
            ILocalStorageService localStorage)
        {
            this._httpClient = httpClient;
            this._authStateProvider = authStateProvider;
            this._localStorage = localStorage;
            this._configuration = configuration;
            //baseUrl = _configuration.GetSection("urls:baseUrlServer").Value!;
            baseUrl = "https://localhost:7218/";
            //baseUrl = "http://192.180.0.192:5656/";
        }

        // returns int if 0 - error   1 - redirect
        public async Task<int> Login(Response inp)
        {
            if (!inp.success)
            {
                return 0;
            }
            
            var userLoginResponse = JsonConvert.DeserializeObject<UserRegisterLogin>(inp.data.ToString());
            //var userLoginResponse = JsonSerializer.Deserialize<UserRegisterLogin>(JsonSerializer.Serialize(inp.data));

            Console.WriteLine(inp.data.ToString());

            //var userResponse = JsonConvert.DeserializeObject<ResponseUser>((userLoginResponse.responseUser).ToString());

            await _localStorage.SetItemAsync("accessToken", userLoginResponse.token);
            token = userLoginResponse.token;
            ((Auth.AuthProvider)_authStateProvider).NotifyUserAuthentication(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return 1;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("accessToken");
            token = "";
            ((Auth.AuthProvider)_authStateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public string GetToken()
        {
            return this.token!;
        }
    }
}
