using Microsoft.EntityFrameworkCore;
using Cuppie.Domain.Entities;
namespace Cuppie.Infrastructure.Data
{
    public class CuppieDbContext (DbContextOptions<CuppieDbContext> options) : DbContext (options)
    {
        public DbSet<UserEntity> User { get; set; } 
        public DbSet<RefreshTokenEntity> RefreshToken { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().ToTable("User");
            modelBuilder.Entity<RefreshTokenEntity>().ToTable("RefreshToken");
        }
    }
}
