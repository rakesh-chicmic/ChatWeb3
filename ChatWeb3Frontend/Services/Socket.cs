using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Blazored.LocalStorage;

namespace ChatWeb3Frontend.Services
{
    public class Socket
    {
        public static HubConnection? hubConnection;
        protected NavigationManager _navMgr;
        private readonly IConfiguration _configuration;
        private readonly ILocalStorageService _localStorage;

        public string baseUrl = "";
        //public string baseUrl = "http://192.180.0.192:5656/";
        public Socket(NavigationManager NavigationManager, IConfiguration configuration, ILocalStorageService localStorage)
        {
            _navMgr = NavigationManager;
            _configuration = configuration;
            //baseUrl = _configuration.GetSection("urls:baseUrlServer").Value!;
            baseUrl = "https://localhost:7218/";
            _localStorage = localStorage;
        }
        public async Task Connect()
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                .WithUrl(_navMgr.ToAbsoluteUri($"{baseUrl}chatHubs"), options =>
                {
                    options.SkipNegotiation = true;
                    options.Transports = HttpTransportType.WebSockets;
                    options.SkipNegotiation = true;
                    //options.AccessTokenProvider = () => Task.FromResult(_authProvider.GetAuthenticationStateAsync);
                    options.AccessTokenProvider = async () =>
                    {
                        //AuthenticationStateProvider authState = authProvider;
                        return await _localStorage.GetItemAsync<string>("accessToken");
                    };
                })
                .Build();
                try
                {
                    await hubConnection.StartAsync();
                    Console.WriteLine("Connection started");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while starting connection: " + ex);
                }

            }

        }

        public HubConnection GetHubConnection()
        {
            return hubConnection!;
        }
    }
}
