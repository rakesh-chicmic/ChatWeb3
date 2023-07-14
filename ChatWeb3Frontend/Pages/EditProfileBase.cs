using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Nethereum.JsonRpc.Client;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Text.Json;
using Tewr.Blazor.FileReader;

namespace ChatWeb3Frontend.Pages
{
    public class EditProfileBase : ComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }
        public UpdateUser updateUser= new UpdateUser();
        public UserResponse userResponse = new UserResponse();
        public APIResponse response = new APIResponse();
        protected override async Task OnInitializedAsync()
        {      
             response = await UserService.GetAsync();
             var resData = JsonSerializer.Serialize(response.Data);
             userResponse = JsonSerializer.Deserialize<UserResponse>(resData);
             updateUser.Username = userResponse.Username;
             updateUser.FirstName = userResponse.FirstName;
             updateUser.LastName = userResponse.LastName;
             updateUser.PathToProfilePic = userResponse.PathToProfilePic;
        }

        protected async Task UpdateUser_Click(UpdateUser update)
        {
            try
            {
                response = await UserService.UpdateAsync(update);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
