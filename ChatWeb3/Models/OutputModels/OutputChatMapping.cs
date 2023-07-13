namespace ChatWeb3.Models
{
    public class OutputChatMappings
    {
        public Guid chatId { get; set; }
        public string senderId { get; set; } = string.Empty;
        public string senderFirstName { get; set; } = string.Empty;
        public string senderLastName { get; set; } = string.Empty;
        public string receiveId { get; set; } = string.Empty;
        public string receiverFirstName { get; set; } = string.Empty;
        public string receiverLastName { get; set; } = string.Empty;
        public DateTime? dateTime { get; set; } = DateTime.Now;
        //public bool IsDeleted { get; set; }

        public OutputChatMappings() { }
        public OutputChatMappings(User sender,User receiver, ChatMappings mapping)
        {
            chatId = mapping.id;
            senderId = sender.id.ToString();
            senderFirstName = sender.firstName;
            senderLastName = sender.lastName;
            receiveId = receiver.id.ToString();
            receiverFirstName = receiver.firstName;
            receiverLastName = receiver.lastName;
            dateTime = mapping.datetime;
        }
    }
}
