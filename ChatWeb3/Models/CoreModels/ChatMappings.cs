namespace ChatWeb3.Models
{
    public class ChatMappings
    {
        public Guid id { get; set; } = Guid.Empty;
        public Guid senderId { get; set; } = Guid.Empty;
        public Guid receiverId { get; set; } = Guid.Empty;
        public DateTime? createdAt { get; set; } = DateTime.MinValue;
        public bool isGroup { get; set; } = false;
        public bool isDeleted { get; set; } = false;

        public ChatMappings() { }
        public ChatMappings(Guid senderId, Guid recevierId, bool isGroup)
        {
            id = Guid.NewGuid();
            this.senderId = senderId;
            this.receiverId = recevierId;
            this.isGroup = isGroup;
            createdAt = DateTime.Now;
            isDeleted = false;
        }
    }
}
