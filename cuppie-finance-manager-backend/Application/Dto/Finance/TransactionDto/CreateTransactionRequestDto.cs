using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Finance.TransactionDto;

public class CreateTransactionRequestDto // dto for controller post methods
{
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    [Required] public bool? Income { get; set; }
    [Required]
    public decimal? Amount { get; set; }
    [Required]
    public long? CategoryId { get; set; }
}