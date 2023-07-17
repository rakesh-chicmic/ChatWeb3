namespace ChatWeb3Frontend.Models
{
    public class FileResponseData
    {
        public ResponseUser user { get; set; } = new ResponseUser();
        public string fileName { get; set; } = string.Empty;
        public string pathToPic { get; set; } = string.Empty;

    }
}
