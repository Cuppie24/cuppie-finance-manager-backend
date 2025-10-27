using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class TransactionEntity
{
    public long Id { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required, Column(TypeName = "numeric(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(50)]
    public string? Comment { get; set; }
    public CategoryEntity Category { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public int UserId { get; set; }

    public TransactionEntity(decimal amount, int categoryId, int userId,  DateTime createdAt)
    {
        Amount = amount;
        CategoryId = categoryId;
        UserId = userId;
        CreatedAt = createdAt;
    }
}