namespace Application.Dto.Finance.TransactionDto;

public class PatchTransactionDto
{
    public long Id { get; set; }
    public string? Comment { get; set; }
    public bool? Income { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal? Amount { get; set; }
    public int? CategoryId { get; set; }
    public int? UserId { get; set; }

    public PatchTransactionDto(long id)
    {
        Id = id;
    }

    public PatchTransactionDto(PatchTransactionRequestDto requestDto, int? userId)
    {
        if(requestDto.Id is null)
            throw new ArgumentNullException(nameof(requestDto.Id));
        Id = requestDto.Id.Value;
        Comment = requestDto.Comment;
        Income = requestDto.Income;
        CreatedAt = requestDto.CreatedAt;
        Amount = requestDto.Amount;
        CategoryId = requestDto.CategoryId;
        UserId = userId;
    }
    
}