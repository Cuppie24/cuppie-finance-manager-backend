using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Finance.TransactionDto;

public class PatchTransactionDto
{
    [Required]
    public long? Id { get; set; }
    public string? Comment { get; set; }
    public bool? Income { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal? Amount { get; set; }
    public int? CategoryId { get; set; }
    public int? UserId { get; set; }
}