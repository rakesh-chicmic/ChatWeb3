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
        public IToastService Toast { get; set; }
        [Inject]
        public IFileUploadService FileUploadService { get; set; }

        public UpdateUser updateUser = new UpdateUser();
        public Response response = new Response();
        public ValidateUsernameModel validateUsername = new ValidateUsernameModel();
        public Action<ChangeEventArgs> onInputDebounced;
        public FileResponseData fileUpload = new FileResponseData();
        public ElementReference elementReference = new ElementReference();
        public string imagePath = "https://cdn-icons-png.flaticon.com/512/1177/1177568.png?w=740&t=st=1689596149~exp=1689596749~hmac=8fc514c8173c4865f99e94e36b1cb77422c6fad5651f4726eab0c29ea4bf8a49";

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
            if (update.username == null)
            {
                Toast.ShowInfo("Please enter Username");
                return;
            }
            else if (update.pathToProfilePic==null)
            {
                Toast.ShowInfo("Please Upload your Profile Picture");
                return;
            }
            else if (update.firstName == null)
            {
                Toast.ShowInfo("Please enter Firstname");
                return;
            }
            else if (update.lastName == null)
            {
                Toast.ShowInfo("Please enter Lastname");
                return;
            }
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
                    Toast.ShowInfo($"Username available");
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
                imagePath = $"http://192.180.0.192:4545/{updateUser.pathToProfilePic}";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
