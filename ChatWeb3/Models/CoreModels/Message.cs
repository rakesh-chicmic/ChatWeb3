namespace ChatWeb3.Models
{
    public class Message
    {
        public Guid id { get; set; } = Guid.Empty;
        public Guid senderId { get; set; } = Guid.Empty;
        public Guid chatId { get; set; } = Guid.Empty;
        public string content { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.MinValue;
        public int type { get; set; } = 1;  //type-1 = file   type-2 = image
        public bool isDeleted { get; set; } = false;
        public bool isSeen { get; set; } = false;
        public string? pathToFileAttachment { get; set; } = string.Empty;
        public Message() { }
        public Message(Guid senderId, Guid chatId, string content, int type, string pathToFileAttachment)
        {
            id = Guid.NewGuid();
            this.senderId = senderId;
            this.chatId = chatId;
            this.content = content;
            this.type = type;
            this.pathToFileAttachment = pathToFileAttachment;
            createdAt = DateTime.Now;
            isDeleted = false;
            isSeen = false;
        }
        public Message(InputMessage inpMsg)
        {
            id = Guid.NewGuid();
            this.senderId = new Guid(inpMsg.senderId);
            this.chatId = new Guid(inpMsg.chatId);
            this.content = inpMsg.content;
            this.type = inpMsg.type;
            this.pathToFileAttachment = inpMsg.pathToFileAttachment;
            createdAt = DateTime.Now;
            isDeleted = false;
            isSeen = false;
        }
    }
}
