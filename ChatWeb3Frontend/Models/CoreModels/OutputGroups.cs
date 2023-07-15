namespace ChatWeb3.Models
{
    public class OutputGroups
    {
        public Guid id { get; set; } = Guid.Empty;
        public string name { get; set; } = string.Empty;
        public DateTime? datetime { get; set; } = DateTime.MinValue;
        public string? pathToProfilePic { get; set; } = string.Empty;
        public OutputGroups() { }
        public OutputGroups(Group grp,DateTime? datetime)
        {
            id = grp.id;
            name = grp.name;
            this.datetime = datetime;
            pathToProfilePic = grp.pathToProfilePic;
        }
    }
}