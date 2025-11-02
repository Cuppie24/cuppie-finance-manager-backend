using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Finance;

public class TransactionEntity
{
    public long Id { get; set; }
    
    [Required]
    public DateTime? CreatedAt { get; set; }
    
    [Required, Column(TypeName = "numeric(18,2)")]
    public decimal Amount { get; set; }
    [Required] public bool Income { get; set; }
    
    [MaxLength(50)]
    public string? Comment { get; set; }

    [Required]
    public long CategoryId { get; set; }
    public CategoryEntity CategoryEntity { get; set; }
    
    [Required]
    public int UserId { get; set; }

    public TransactionEntity(decimal amount, long categoryId, int userId,  DateTime? createdAt, string? comment, bool income)
    {
        Amount = amount;
        CategoryId = categoryId;
        UserId = userId;
        CreatedAt = createdAt;
        Comment = comment;
        Income = income;
    }

    public TransactionEntity()
    {
    }
}