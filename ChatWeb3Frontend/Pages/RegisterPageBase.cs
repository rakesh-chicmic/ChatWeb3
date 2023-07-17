using Blazored.Toast.Services;
using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace ChatWeb3Frontend.Pages
{
    public class RegisterPageBase : ComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService Toast {  get ; set; }
        [Inject]
        public IFileUploadService FileUploadService { get; set; }

        public UpdateUser updateUser = new UpdateUser();
        public Response response = new Response();
        public ValidateUsernameModel validateUsername = new ValidateUsernameModel();
        public Action<ChangeEventArgs> onInputDebounced;
        public FileResponseData fileUpload = new FileResponseData();  
        public ElementReference elementReference = new ElementReference();

        protected override void OnInitialized()
        {
            onInputDebounced = DebounceEvent<ChangeEventArgs>(e => updateUser.username = (string)e.Value, TimeSpan.FromSeconds(1));
            base.OnInitialized();
        }
        Action<T> DebounceEvent<T>(Action<T> action, TimeSpan interval)
        {
            return Debounce<T>(arg =>
            {
                InvokeAsync(async () =>
                {
                    action(arg);
                    await ValidateUsername_Click(updateUser.username);
                    StateHasChanged();
                });
            }, interval);
        }

        Action<T> Debounce<T>(Action<T> action, TimeSpan interval)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var last = 0;
            return arg =>
            {
                var current = System.Threading.Interlocked.Increment(ref last);
                Task.Delay(interval).ContinueWith(task =>
                {
                    if (current == last)
                    {
                        action(arg);
                    }
                });
            };
        }
        protected async Task UpdateUser_Click(UpdateUser update)
        {
            try
            {
                response = await UserService.UpdateAsync(update);
                if (response.success)
                {
                    Toast.ShowSuccess("User Details Updated");
                    NavigationManager.NavigateTo("/home");
                }
              
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected async Task ValidateUsername_Click(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                try
                {
                    response = await UserService.ValidateUsernameAsync(username);
                    var resData = JsonSerializer.Serialize(response.data);
                    validateUsername = JsonSerializer.Deserialize<ValidateUsernameModel>(resData);
                    if (!validateUsername.isAvailable)
                    {
                        Toast.ShowInfo($"Username already exist. Suggested:{validateUsername.suggestedUsername}");
                    }
                    await InvokeAsync(StateHasChanged);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        protected async Task UploadProfileImage_Click(ElementReference elementReference)
        {
            try
            {
                response = await FileUploadService.UploadFileAsync(elementReference);
                if (response.statusCode == 200)
                {
                    Toast.ShowSuccess("Profile Picture Uploaded");
                }
                var resData = JsonSerializer.Serialize(response.data);
                fileUpload = JsonSerializer.Deserialize<FileResponseData>(resData);
                updateUser.pathToProfilePic = fileUpload.pathToPic;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
