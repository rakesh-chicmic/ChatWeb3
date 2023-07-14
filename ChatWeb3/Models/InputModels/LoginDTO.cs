namespace ChatWeb3.Models
{
    public class LoginDTO
    {
        public string signer { get; set; } = string.Empty; // Ethereum account that claim the signature
        public string signature { get; set; } = string.Empty; // The signature
        public string message { get; set; } = string.Empty; // The plain message
        public string hash { get; set; } = string.Empty; // The prefixed and sha3 hashed message 
    }
}
