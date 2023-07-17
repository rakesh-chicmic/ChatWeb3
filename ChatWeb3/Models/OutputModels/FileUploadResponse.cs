namespace ChatWeb3Frontend.Models
{
    public class FileUploadResponse
    {
        public string fileName { get; set; } = string.Empty;
        public string pathToFile { get; set; } = string.Empty;

        public FileUploadResponse(string fileName, string pathToFile)
        {
            this.fileName = fileName;
            this.pathToFile = pathToFile;
        }
    }
}

// response data after uploading a new profile pic