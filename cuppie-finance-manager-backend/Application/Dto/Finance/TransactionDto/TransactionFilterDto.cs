namespace Application.Dto.Finance.TransactionDto;

public class TransactionFilterDto
{
    public int? UserId { get; set; }
    public bool? Income { get; set; }
    public long[]? CategoryIdList { get; set; }
    public DateTime? From { get; set; } 
    public DateTime? To { get; set; } 
}