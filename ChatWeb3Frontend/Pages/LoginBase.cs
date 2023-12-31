﻿using MetaMask.Blazor;
using MetaMask.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;


namespace ChatWeb3Frontend.Pages
{
    public class LoginBase : ComponentBase
    {
        //[Inject]
        //public Microsoft.JSInterop.JSRuntime jsRuntime { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IMetaMaskService MetaMaskService { get; set; } = default!;
        public bool HasMetaMask { get; set; }
        public string? SelectedAddress { get; set; }
        public string? TransactionCount { get; set; }
        public string? InitializeMsg { get; set; }
        protected override async Task OnInitializedAsync()
        {
            HasMetaMask = await MetaMaskService.HasMetaMask();
            if (HasMetaMask)
                await MetaMaskService.ListenToEvents();

            bool isSiteConnected = await MetaMaskService.IsSiteConnected();
            if (isSiteConnected)
            {
                await GetSelectedAddress();
            }
        }
        
        public async Task ConnectMetaMask()
        {
            try
            {
                //var res = await jsRuntime.InvokeAsync<object>("web3Login", null);
                //await MetaMaskService.ConnectMetaMask();
                //Console.WriteLine(res);
                //await GetSelectedAddress();
            }
            catch (UserDeniedException)
            {
                InitializeMsg = "User Denied";
            }
            catch (Exception ex)
            {
                InitializeMsg = ex.ToString();
            }
            NavigationManager.NavigateTo("/details");
        }

        public async Task GetSelectedAddress()
        {
            SelectedAddress = await MetaMaskService.GetSelectedAddress();
        }

        public async Task GetTransactionCount()
        {
            var transactionCount = await MetaMaskService.GetTransactionCount();
            TransactionCount = transactionCount.ToString();
        }

    }
}
