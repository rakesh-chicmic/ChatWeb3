using Blazored.Toast.Services;
using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Nethereum.JsonRpc.Client;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Text.Json;
using Tewr.Blazor.FileReader;

namespace ChatWeb3Frontend.Pages
{
    public class UpdateProfileBase : ComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService Toast { get; set; }
        [Inject]
        public IFileUploadService FileUploadService { get; set; }
        public UpdateUser updateUser= new UpdateUser();
        public ResponseUser userResponse = new ResponseUser();
        public Response response = new Response();
        public string imagePath = null;
        public FileResponseData fileUpload = new FileResponseData();
        public ElementReference elementReference = new ElementReference();
        protected override async Task OnInitializedAsync()
        {      
             response = await UserService.GetAsync();
             var resData = JsonSerializer.Serialize(response.data);
             userResponse = JsonSerializer.Deserialize<ResponseUser>(resData)!;
             updateUser.username = userResponse.username;
             updateUser.firstName = userResponse.firstName;
             updateUser.lastName = userResponse.lastName;
             updateUser.pathToProfilePic = userResponse.pathToProfilePic!;
             imagePath = $"http://192.180.0.192:4545/{updateUser.pathToProfilePic}";
        }

        protected async Task UpdateUser_Click(UpdateUser update)
        {
            try
            {
                response = await UserService.UpdateAsync(update);
                if (response.success)
                {
                    Toast.ShowSuccess("Profile Updated");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected async Task UploadProfileImage_Click(ElementReference elementReference)
        {
            try
            {
                response = await FileUploadService.UploadFileAsync(elementReference);
                if (response.statusCode == 200)
                {
                    Toast.ShowSuccess("Profile Picture Updated");
                }
                var resData = JsonSerializer.Serialize(response.data);
                fileUpload = JsonSerializer.Deserialize<FileResponseData>(resData);
                updateUser.pathToProfilePic = fileUpload.pathToPic;
                imagePath = $"http://192.180.0.192:4545/{updateUser.pathToProfilePic}";
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
