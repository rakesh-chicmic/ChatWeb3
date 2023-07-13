namespace ChatWeb3.Models
{
    public class InputMessage
    {
        //public string senderId { get; set; } = string.Empty;
        public string chatId { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public int type { get; set; } = 1;  //type-1 = file   type-2 = image
        public bool isGroup { get; set; } = false;
        public string? pathToFileAttachment { get; set; } = string.Empty;
        public InputMessage() { }
    }
}
