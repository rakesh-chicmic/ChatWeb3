namespace ChatWeb3.Models
{
    public class OutputChatMappings
    {
        public Guid chatId { get; set; }
        public string id { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public DateTime? dateTime { get; set; } = DateTime.Now;
        public bool isOnline { get; set; } = false;
        public string pathToProfilePic { get; set; } = string.Empty;

        public OutputChatMappings() { }
        public OutputChatMappings(User receiver, ChatMappings mapping)
        {
            chatId = mapping.id;
            id = receiver.id.ToString();
            firstName = receiver.firstName;
            lastName = receiver.lastName;
            dateTime = mapping.datetime;
            isOnline = receiver.isOnline;
            pathToProfilePic = receiver.pathToProfilePic;
        }
    }
}

// response data for list of chats a user is engaged with