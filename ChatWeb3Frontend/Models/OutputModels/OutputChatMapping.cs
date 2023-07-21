namespace ChatWeb3Frontend.Models
{
    public class OutputChatMapping
    {
        public Guid chatId { get; set; }
        public string id { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public DateTime? dateTime { get; set; } = DateTime.Now;
        public bool isOnline { get; set; } = false;
        public string pathToProfilePic { get; set; } = string.Empty;
        public int countOfUnseen { get; set; } = 0;

        public OutputChatMapping() { }
        public OutputChatMapping(User receiver, ChatMappings mapping, int countOfUnseen)
        {
            chatId = mapping.id;
            username = receiver.username;
            id = receiver.id.ToString();
            dateTime = mapping.datetime;
            isOnline = receiver.isOnline;
            pathToProfilePic = receiver.pathToProfilePic;
            this.countOfUnseen = countOfUnseen;
        }
    }
}

// response data for list of chats a user is engaged with