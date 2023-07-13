namespace ChatWeb3.Models
{
    public class OutputChatMappings
    {
        public Guid chatId { get; set; }
        //public string senderId { get; set; } = string.Empty;
        //public string senderFirstName { get; set; } = string.Empty;
        //public string senderLastName { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public DateTime? dateTime { get; set; } = DateTime.Now;
        public bool isOnline { get; set; }

        public OutputChatMappings() { }
        public OutputChatMappings(User receiver, ChatMappings mapping)
        {
            chatId = mapping.id;
            //senderId = sender.id.ToString();
            //senderFirstName = sender.firstName;
            //senderLastName = sender.lastName;
            id = receiver.id.ToString();
            firstName = receiver.firstName;
            lastName = receiver.lastName;
            dateTime = mapping.datetime;
            isOnline = receiver.isOnline;
        }
    }
}
