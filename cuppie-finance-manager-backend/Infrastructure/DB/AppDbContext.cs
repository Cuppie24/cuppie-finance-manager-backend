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
        var deletedCategories = ChangeTracker.Entries<CategoryEntity>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity.Id)
            .Distinct()
            .ToList();

        if (!deletedCategories.Any()) return;

        var defaultExists = Categories.Any(c => c.Id == DefaultCategoryId);
        if (!defaultExists)
        {
            throw new InvalidOperationException($"Default category with transactionId={DefaultCategoryId} does not exist.");
        }

        // Update transactions that reference the deleted categories
        foreach (var categoryId in deletedCategories)
        {
            var transactionsToUpdate = Transactions
                .Where(t => t.CategoryId == categoryId)
                .ToList();

            foreach (var transaction in transactionsToUpdate)
            {
                transaction.CategoryId = DefaultCategoryId;
            }
        }
    }

}