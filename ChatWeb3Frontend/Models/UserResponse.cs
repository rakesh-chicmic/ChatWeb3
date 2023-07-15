namespace ChatWeb3Frontend.Models
{
    public class UserResponse
    {
        public Guid id { get; set; } = Guid.Empty;
        public string accountAddress { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.MinValue;
        public DateTime? updatedAt { get; set; } = DateTime.MinValue;
        public string? pathToProfilePic { get; set; } = string.Empty;
        public bool isOnline { get; set; } = false;
    }
}
