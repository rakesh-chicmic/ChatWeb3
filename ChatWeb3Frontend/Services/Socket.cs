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

        public string baseUrl = "";
        //public string baseUrl = "http://192.180.0.192:5656/";
        public Socket(NavigationManager NavigationManager, IConfiguration configuration)
        {
            _navMgr = NavigationManager;
            _configuration = configuration;
            //baseUrl = _configuration.GetSection("urls:baseUrlServer").Value!;
            baseUrl = "https://localhost:7218/";
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
                        return "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJiYTRkNjBkZC0wMmJhLTQxODAtYjQ3Yy1jNjBlMDFiN2FjMjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoic2RzYSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3NpZCI6IjB4NmQ3NjM2NDg2YjBEZDJiMWEzNEJmOThBZDQwNzhBY0MxNzQxZkM3MiIsImV4cCI6MTY4OTQ4Mjg2Nn0.u7wconC_82oPBSypie2P3ZnrpRNvv-u70aZbVMtCDkLqgavRMdnf4qCwQGoT31SMFTR1jKF0ueigo9rf4I-tdA";
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
