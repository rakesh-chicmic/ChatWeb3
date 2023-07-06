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
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatMappings> ChatMappings { get; set; }
    }
}
