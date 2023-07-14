namespace ChatWeb3Frontend.Models
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string AccountAddress { get; set; }
        public string Username { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public DateTime? CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public string? PathToProfilePic { get; set; } 
        public bool IsOnline { get; set; } 
    }
}
