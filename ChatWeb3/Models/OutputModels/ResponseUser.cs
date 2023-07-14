namespace ChatWeb3.Models
{
    public class ResponseUser
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

        public ResponseUser() { }

        public ResponseUser(User user)
        {
            username = user.username;
            accountAddress = user.accountAddress;
            firstName = user.firstName;
            lastName = user.lastName;
            pathToProfilePic = user.pathToProfilePic;
            id = user.id;
            createdAt = user.createdAt;
            updatedAt = user.updatedAt;
            isOnline = user.isOnline;
        }
        public ResponseUser(Guid id, string username, string accountAddress, string firstName, string lastName, string pathToProfilePic, DateTime createdAt, DateTime updatedAt)
        {
            this.id = id;
            this.username = username;
            this.accountAddress = accountAddress;
            this.firstName = firstName;
            this.lastName = lastName;
            this.pathToProfilePic = pathToProfilePic;
            this.createdAt = createdAt;
            this.updatedAt = updatedAt;
        }
    }
}

/// response data for sending user details in order to hide security/ inner user details