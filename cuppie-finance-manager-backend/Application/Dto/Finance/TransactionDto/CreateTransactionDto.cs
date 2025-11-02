using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Finance.TransactionDto;

public class CreateTransactionDto
{
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    [Required] public bool? Income { get; set; }
    [Required]
    public decimal? Amount { get; set; }
    [Required]
    public long? CategoryId { get; set; }
    [Required]
    public int? UserId { get; set; }
}