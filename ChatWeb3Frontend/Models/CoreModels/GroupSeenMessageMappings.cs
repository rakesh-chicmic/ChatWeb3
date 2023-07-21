namespace ChatWeb3Frontend.Models
{
    public class GroupSeenMessageMappings
    {
        public Guid id { get; set; } = Guid.Empty;
        public Guid messageId { get; set; } = Guid.Empty;
        public Guid groupId { get; set; } = Guid.Empty;
        public Guid userId { get; set; } = Guid.Empty;
        public GroupSeenMessageMappings(){}
        public GroupSeenMessageMappings(Guid messageId, Guid groupId, Guid userId)
        {
            id = Guid.NewGuid();
            this.messageId = messageId;
            this.groupId = groupId;
            this.userId = userId;
        }
    }
}
