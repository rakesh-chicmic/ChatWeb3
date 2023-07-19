namespace ChatWeb3.Models
{
    public class UserRegisterLogin
    {
        public ResponseUser responseUser { get; set; } = new ResponseUser();
        public string token { get; set; } = string.Empty;

        public UserRegisterLogin() { }
        public UserRegisterLogin(ResponseUser responseUser)
        {
            this.responseUser = responseUser;
        }
    }
}

//// response data when a user logins/register into the system