namespace ChatWeb3Frontend.Models
{
    public class ValidateUsernameModel
    {
        public string username { get; set; } = string.Empty;
        public bool isAvailable { get; set; } = false;
        public string suggestedUsername { get; set; } = string.Empty;
    }
}
