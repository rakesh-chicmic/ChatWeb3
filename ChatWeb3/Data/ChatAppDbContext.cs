using ChatWeb3.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWeb3.Data
{
    public class ChatAppDbContext : DbContext
    {
        public ChatAppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
