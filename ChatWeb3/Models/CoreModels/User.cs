namespace ChatWeb3.Models
{
    public class User
    {
        public Guid id { get; set; } = Guid.Empty;
        public string username { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.MinValue;
        public DateTime? updatedAt { get; set; } = DateTime.MinValue;
        public bool isDeleted { get; set; } = false;
        public bool isActive { get; set; } = false;
        public bool isOnline { get; set; } = false;
        public string? pathToProfilePic { get; set; } = string.Empty;

        public User() { }
        public User(string username, string firstName, string lastName, string pathToProfilePic)
        {
            this.username = username;
            this.firstName = firstName;
            this.lastName = lastName;
            this.pathToProfilePic = pathToProfilePic;
            id = Guid.NewGuid();
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
            isActive = true;
            isOnline = true;
            isDeleted = false;
        }
    }
}

