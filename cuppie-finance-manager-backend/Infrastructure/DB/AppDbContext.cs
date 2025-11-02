using Application.Interfaces.Repositories;
using Domain.Entities.Finance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionEntity>(entity =>
        {
            entity
                .HasOne(t => t.CategoryEntity)
                .WithMany(t => t.Transactions)
                .HasForeignKey(t => t.CategoryId);
        });
    }
    
    private const int DefaultCategoryId = 1;

    public override int SaveChanges()
    {
        HandleCategoryDeletion();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleCategoryDeletion();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void HandleCategoryDeletion()
    {
        var deletedCategoryIds = ChangeTracker.Entries<CategoryEntity>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity.Id)
            .Distinct()
            .ToList();

        if (!deletedCategoryIds.Any()) return;

        var defaultExists = Categories.Any(c => c.Id == DefaultCategoryId);
        if (!defaultExists)
        {
            throw new InvalidOperationException($"Default category with id={DefaultCategoryId} does not exist.");
        }

        var idsParam = string.Join(",", deletedCategoryIds);

        Database.ExecuteSqlRaw(
            $"UPDATE \"Transactions\" SET \"CategoryId\" = {{0}} WHERE \"CategoryId\" IN ({idsParam})",
            DefaultCategoryId
        );

        ChangeTracker.Clear();
    }

}