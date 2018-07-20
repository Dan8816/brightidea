using Microsoft.EntityFrameworkCore;//add this dependency

namespace DojoSecret.Models
{
    public class YourContext : DbContext
    {
        public YourContext(DbContextOptions<YourContext> options) : base(options) { }
        public DbSet<User> ninjas { get; set; } //Users = the table name
        //<Person> is the class model that will link to the database
        public DbSet<Message> secrets { get; set; }
        public DbSet<Like> likes { get; set; }
    }
}