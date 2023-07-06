namespace ChatWeb3.Models
{
    public class ResponseUser
    {
        public Guid id { get; set; } = Guid.Empty;
        public string username { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.MinValue;
        public DateTime? updatedAt { get; set; } = DateTime.MinValue;
        public string? pathToProfilePic { get; set; } = string.Empty;

        public ResponseUser(User user)
        {
            this.username = user.username;
            this.firstName = user.firstName;
            this.lastName = user.lastName;
            this.pathToProfilePic = user.pathToProfilePic;
            id = user.id;
            createdAt = user.createdAt;
            updatedAt = user.updatedAt;
        }
    }
}
