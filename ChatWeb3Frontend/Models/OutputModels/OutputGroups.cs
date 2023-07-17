namespace ChatWeb3Frontend.Models
{
    public class OutputGroups
    {
        public Guid id { get; set; } = Guid.Empty;
        public Guid chatId { get; set; } = Guid.Empty;
        public string name { get; set; } = string.Empty;
        public DateTime? datetime { get; set; } = DateTime.MinValue;
        public string? pathToProfilePic { get; set; } = string.Empty;
        public int countOfUnseen { get; set; } = 0;
        public OutputGroups() { }
        public OutputGroups(Group grp,ChatMappings cm, int countOfUnseen)
        {
            id = grp.id;
            chatId = cm.id;
            name = grp.name;
            this.datetime = cm.datetime;
            pathToProfilePic = grp.pathToProfilePic;
            this.countOfUnseen = countOfUnseen;
        }
    }
}

//// response data for a list of groups a user is part of