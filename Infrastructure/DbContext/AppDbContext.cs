using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext;

public class AppDbContext(DbContextOptions<AppDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionEntity>(entity =>
        {
            entity
                .HasOne(t => t.Category)
                .WithMany(t => t.Transactions)
                .HasForeignKey(t => t.CategoryId);
        });
    }
}