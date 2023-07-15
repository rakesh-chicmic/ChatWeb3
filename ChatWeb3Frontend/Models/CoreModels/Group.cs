namespace ChatWeb3.Models
{
    public class Group
    {
        public Guid id { get; set; } = Guid.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.MinValue;
        public DateTime? updatedAt { get; set; } = DateTime.MinValue;
        public Guid adminId { get; set; } = Guid.Empty;
        public bool isDeleted { get; set; } = false;
        public string? pathToProfilePic { get; set; } = string.Empty;
        public int noOfParticipants { get; set; } = 0;

        public Group() { }
        public Group(string name, string description, Guid adminId, string pathToProfilePic, int noOfParticipants)
        {
            id = Guid.NewGuid();
            this.name = name;
            this.description = description;
            this.adminId = adminId;
            this.noOfParticipants = noOfParticipants;
            this.pathToProfilePic = pathToProfilePic;
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
            isDeleted = false;
        }
    }
}
