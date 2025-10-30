using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class CategoryEntity(string? name)
{
    public long Id { get; set; }
    [Required, MaxLength(20)] public string Name { get; set; } = null!;

    public List<TransactionEntity>? Transactions { get; set; } = new();
}