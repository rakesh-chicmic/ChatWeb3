using Microsoft.EntityFrameworkCore;

namespace ChatWeb3Frontend.Models
{
    public class UpdateUser
    {
        public string username { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string pathToProfilePic { get; set; } = string.Empty;
    }
}

