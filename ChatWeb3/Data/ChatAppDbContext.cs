using ChatWeb3Frontend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWeb3Frontend.Data
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
        public DbSet<AccountMessageMapping> AccountMessagemappings { get; set; }
        public DbSet<GroupSeenMessageMappings> GroupSeenMessageMappings { get; set; }
    }
}
