using Blazored.Toast.Services;
using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Models.InputModels;
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
        public string imagePath = "https://cdn-icons-png.flaticon.com/512/1177/1177568.png?w=740&t=st=1689596149~exp=1689596749~hmac=8fc514c8173c4865f99e94e36b1cb77422c6fad5651f4726eab0c29ea4bf8a49";
        public FileResponseData fileUpload = new FileResponseData();
        public ElementReference profileImageReference = new ElementReference();
        public bool isInputDisabled = true;
        public Action<ChangeEventArgs> isUsernameExists;
        protected override async Task OnInitializedAsync()
        {      
             response = await UserService.GetAsync();
             var resData = JsonSerializer.Serialize(response.data);
             userResponse = JsonSerializer.Deserialize<ResponseUser>(resData)!;
             updateUser.username = userResponse.username;
             updateUser.pathToProfilePic = userResponse.pathToProfilePic!;
             imagePath = $"http://192.180.0.192:4545/{updateUser.pathToProfilePic}";
        }

        protected async Task EnableInput()
        {
            isInputDisabled = false;
        }

        protected async Task UpdateUser_Click(UpdateUser update)
        {
            isInputDisabled = true;
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

        protected async Task UploadImage_Click(ElementReference profileImageReference)
        {
            try
            {
                response = await FileUploadService.UploadFileAsync(profileImageReference);
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
