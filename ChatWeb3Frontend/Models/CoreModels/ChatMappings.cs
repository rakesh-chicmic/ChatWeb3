namespace ChatWeb3.Models
{
    public class ChatMappings
    {
        public Guid id { get; set; } = Guid.Empty;
        public Guid senderId { get; set; } = Guid.Empty;
        public Guid receiverId { get; set; } = Guid.Empty;
        public DateTime? datetime { get; set; } = DateTime.MinValue;
        public bool isGroup { get; set; } = false;
        public bool isDeleted { get; set; } = false;

        public ChatMappings() { }
        public ChatMappings(Guid senderId, Guid recevierId, bool isGroup)
        {
            id = Guid.NewGuid();
            this.senderId = senderId;
            this.receiverId = recevierId;
            this.isGroup = isGroup;
            datetime = DateTime.Now;
            isDeleted = false;
        }
    }
}
//mapping b/w diff users or b/w user and group 
// receiverId can be another user or groupId