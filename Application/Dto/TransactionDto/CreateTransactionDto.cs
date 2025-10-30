using System.ComponentModel.DataAnnotations;

namespace Application.Dto.TransactionDto;

public class CreateTransactionDto
{
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    [Required]
    public decimal? Amount { get; set; }
    [Required]
    public long? CategoryId { get; set; }
    [Required]
    public long? UserId { get; set; }
}