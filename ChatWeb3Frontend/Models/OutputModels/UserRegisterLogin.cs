namespace ChatWeb3Frontend.Models
{
    public class UserRegisterLogin
    {
        public ResponseUser responseUser { get; set; } = new ResponseUser();
        public bool isAlreadyRegistered { get; set; } = false;
        public string token { get; set; } = string.Empty;

        public UserRegisterLogin() { }
        public UserRegisterLogin(ResponseUser responseUser,bool isAlreadyRegistered)
        {
            this.isAlreadyRegistered = isAlreadyRegistered;
            this.responseUser = responseUser;
        }
    }
}
