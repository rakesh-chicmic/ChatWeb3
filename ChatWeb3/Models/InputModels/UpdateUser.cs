using Microsoft.EntityFrameworkCore;

namespace ChatWeb3.Models
{
    public class UpdateUser
    {
        public string username { get; set; } = string.Empty;
        public string pathToProfilePic { get; set; } = string.Empty;
    }
}

