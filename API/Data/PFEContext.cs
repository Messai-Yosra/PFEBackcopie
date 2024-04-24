using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PFEContext : DbContext
    {
        public PFEContext(DbContextOptions<PFEContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = default!;
    }
}
