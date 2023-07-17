namespace ChatWeb3Frontend.Models
{
    public class ResponseGroup
    {
        public Guid id { get; set; } = Guid.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.MinValue;
        public DateTime updatedAt { get; set; } = DateTime.MinValue;
        public Guid adminId { get; set; } = Guid.Empty;
        public string? pathToProfilePic { get; set; } = string.Empty;
        public int noOfParticipants { get; set; } = 0;

        public ResponseGroup() { }
        public ResponseGroup(Group grp)
        {
            this.id = grp.id;
            this.name = grp.name;
            this.description =grp. description;
            this.adminId = grp.adminId;
            this.noOfParticipants = grp.noOfParticipants;
            this.pathToProfilePic = grp.pathToProfilePic;
            createdAt = grp.createdAt;
            updatedAt = grp.updatedAt;
        }
    }
}

// response data for fetching a particular group details