using ChatWeb3Frontend.Models;
using ChatWeb3Frontend.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace ChatWeb3Frontend.Pages
{
    public class UserDetailsBase : ComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }
        public UpdateUser UpdateUser = new UpdateUser();
        public APIResponse Response = new APIResponse();
        protected override void OnInitialized()
        {
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
    }
}
