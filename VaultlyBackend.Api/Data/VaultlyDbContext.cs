using Microsoft.EntityFrameworkCore;
using VaultlyBackend.Api.Entites;

namespace VaultlyBackend.Api.Data
{
    public class VaultlyDbContext(DbContextOptions<VaultlyDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
