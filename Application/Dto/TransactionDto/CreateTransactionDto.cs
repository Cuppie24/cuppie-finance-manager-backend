namespace Application.Dto.TransactionDto;

public class CreateTransactionDto
{
    public string? Comment { get; set; }
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
}