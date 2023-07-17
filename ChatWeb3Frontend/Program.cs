using Blazored.Toast;
using ChatWeb3Frontend;
using ChatWeb3Frontend.Services;
using ChatWeb3Frontend.Services.Contracts;
using MetaMask.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tewr.Blazor.FileReader;
using Microsoft.AspNetCore.Components.Authorization;
using ChatWeb3Frontend.Auth;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://192.180.0.192:4545/") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMetaMaskBlazor();
builder.Services.AddBlazoredToast();
builder.Services.AddFileReaderService();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<Socket>();

await builder.Build().RunAsync();

