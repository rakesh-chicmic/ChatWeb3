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
             response = await UserService.GetYourselfAsync();
             var resData = JsonSerializer.Serialize(response.data);
             userResponse = JsonSerializer.Deserialize<UserResponse>(resData);
             updateUser.username = userResponse.username;
             updateUser.firstName = userResponse.firstName;
             updateUser.lastName = userResponse.lastName;
             updateUser.pathToProfilePic = userResponse.pathToProfilePic;
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
