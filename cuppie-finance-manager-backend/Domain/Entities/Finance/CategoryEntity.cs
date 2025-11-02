using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Finance;

public class CategoryEntity(string? name, bool? income)
{
    public long Id { get; set; }
    [Required, MaxLength(20)] public string? Name { get; set; } = name;
    [Required] public bool? Income { get; set; } = income;

    public List<TransactionEntity> Transactions { get; set; } = new();
}