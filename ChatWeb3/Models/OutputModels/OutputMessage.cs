namespace ChatWeb3.Models
{
    public class OutputMessage
    {
        public Guid id { get; set; } = Guid.Empty;
        public Guid senderId { get; set; } = Guid.Empty;
        public Guid chatId { get; set; } = Guid.Empty;
        public string content { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.MinValue;
        public int type { get; set; } = 1;  //type-1 = file   type-2 = image
        public string? pathToFileAttachment { get; set; } = string.Empty;
        public OutputMessage() { }
        public OutputMessage(Message msg)
        {
            id = msg.id;
            senderId = msg.senderId;
            chatId = msg.chatId;
            content = msg.content;
            type = msg.type;
            pathToFileAttachment = msg.pathToFileAttachment;
            createdAt = msg.createdAt;
        }
    }
}
