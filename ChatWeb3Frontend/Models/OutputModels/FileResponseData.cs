namespace ChatWeb3.Models
{
    public class FileResponseData
    {
        public ResponseUser user { get; set; }
        public string fileName { get; set; } = string.Empty;
        public string pathToPic { get; set; } = string.Empty;

        public FileResponseData(ResponseUser user, string fileName, string pathToPic)
        {
            this.user = user;
            this.fileName = fileName;
            this.pathToPic = pathToPic;
        }
    }
}
