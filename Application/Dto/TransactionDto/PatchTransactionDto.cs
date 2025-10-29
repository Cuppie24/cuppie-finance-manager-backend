namespace Application.Dto.TransactionDto;

public class PatchTransactionDto
{
    public int Id { get; set; }
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal? Amount { get; set; }
    public int? CategoryId { get; set; }
    public int? UserId { get; set; }
}