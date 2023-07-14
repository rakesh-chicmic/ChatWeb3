namespace ChatWeb3Frontend.Models
{
    public class APIResponse
    {
        public int statusCode { get; set; } = 200;
        public string message { get; set; } = "Ok";
        public UserResponse data { get; set; }
        public bool success { get; set; } = true;
    }
}
