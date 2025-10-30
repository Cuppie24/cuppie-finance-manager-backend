using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class TransactionEntity
{
    public long Id { get; set; }
    
    [Required]
    public DateTime? CreatedAt { get; set; }
    
    [Required, Column(TypeName = "numeric(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(50)]
    public string? Comment { get; set; }
    // public CategoryEntity CategoryEntity { get; set; }

    [Required]
    public long CategoryId { get; set; }
    
    [Required]
    public long UserId { get; set; }

    public TransactionEntity(decimal amount, long categoryId, long userId,  DateTime createdAt, string? comment)
    {
        Amount = amount;
        CategoryId = categoryId;
        UserId = userId;
        CreatedAt = createdAt;
        Comment = comment;
    }

}