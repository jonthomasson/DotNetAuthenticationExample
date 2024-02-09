using Microsoft.EntityFrameworkCore;

namespace DotNetAuthentication.Models
{
    public class DotNetAuthenticationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public string DbPath { get; }


        public DotNetAuthenticationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
