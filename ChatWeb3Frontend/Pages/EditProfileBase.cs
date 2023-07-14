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
        //[Inject]
        //public IFileUploadService FileUploadService { get; set; }
        public UpdateUser UpdateUser= new UpdateUser();
        public UserResponse userResponse = new UserResponse();
        public APIResponse Response = new APIResponse();
        protected override async Task OnInitializedAsync()
        {      
             Response = await UserService.GetYourselfAsync();
             var resData = JsonSerializer.Serialize(Response.data);
             userResponse = JsonSerializer.Deserialize<UserResponse>(resData);
            UpdateUser.Username = userResponse.username;
            UpdateUser.FirstName = userResponse.firstName;
            UpdateUser.LastName = userResponse.lastName;
            UpdateUser.PathToProfilePic = userResponse.pathToProfilePic;
        }

        protected async Task UpdateUser_Click(UpdateUser update)
        {
            try
            {
                Response = await UserService.UpdateAsync(update);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //protected async Task OpenFileAsync()
        //{
        //    var file = (await fileReader.CreateReference(elementReference).EnumerateFilesAsync()).FirstOrDefault();
        //    if (file == null) return;
        //    var fileInfo = await file.ReadFileInfoAsync();
        //    fileName = fileInfo.Name;
        //    size = $"{fileInfo.Size}b";
        //    type = fileInfo.Type;

        //    using (var memoryStream = await file.CreateMemoryStreamAsync((int)fileInfo.Size))
        //    {
        //        fileStream = new MemoryStream(memoryStream.ToArray());
        //    }

        //}

        //protected async Task UploadFileAsync()
        //{
        //    string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ByaW1hcnlzaWQiOiJiYTRkNjBkZC0wMmJhLTQxODAtYjQ3Yy1jNjBlMDFiN2FjMjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZmlyc3ROYW1lIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMHg2ZDc2MzY0ODZiMERkMmIxYTM0QmY5OEFkNDA3OEFjQzE3NDFmQzcyIiwiZXhwIjoxNjg5MzI3MzIyfQ.Tb7L2n_xjlc6EKyxXiMB7mfRCjY4LOvlm2ZFxo0L00wqVxUOPoBUM42zUjFS7f98a0QJYFEKkBvtKdBlui01LQ";
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    // string url = "http://192.180.0.192:4545";
        //    var content = new MultipartFormDataContent();
        //    content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data");
        //    content.Add(new StreamContent(fileStream, (int)fileStream.Length), "image", fileName);
        //    var response = await client.PostAsync("api/v1/uploadProfilePic", content);
        //    message = " Upload Success";
        //}

        //protected async Task UploadProfileImage_Click()
        //{
        //    file = UpdateUser.PathToProfilePic;
        //    try
        //    {
        //        Response = await FileUploadService.UploadProfileImageAsync(file);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
