using Microsoft.EntityFrameworkCore;
using VaultlyBackend.Api.Models.Entites;

namespace VaultlyBackend.Api.Data
{
    public class VaultlyDbContext(DbContextOptions<VaultlyDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }
    }
}
