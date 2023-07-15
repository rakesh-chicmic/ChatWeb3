namespace ChatWeb3Frontend.Models.Dtos
{
    public class LoginDTO
    {
        public string Signer { get; set; } 
        public string Signature { get; set; } 
        public string Message { get; set; } 
        public string Hash { get; set; } 
    }
}
