namespace ChatWeb3Frontend.Models
{
    public class Response
    {
        public int statusCode { get; set; } 
        public string message { get; set; } 
        public object data { get; set; }
        public bool success { get; set; } 
    }
}
