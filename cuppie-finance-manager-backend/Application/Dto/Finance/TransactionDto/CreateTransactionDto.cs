using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Finance.TransactionDto;

public class CreateTransactionDto // dto for service post methods (not controller)
{
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool Income { get; set; }
    public decimal Amount { get; set; }
    public long CategoryId { get; set; }
    public int UserId { get; set; }

    public CreateTransactionDto(
        string comment,
        DateTime createdAt,
        bool income,
        decimal amount,
        long categoryId,
        int userId)
    {
        Comment = comment;
        CreatedAt = createdAt;
        Income = income;
        Amount = amount;
        CategoryId = categoryId;
        UserId = userId;
    }

    public CreateTransactionDto(CreateTransactionRequestDto requestDto, int userId)
    {
        Comment = requestDto.Comment;
        CreatedAt = requestDto.CreatedAt;
        if (requestDto.Income is null)
            throw new ArgumentNullException(nameof(requestDto.Income));
        Income = requestDto.Income.Value;
        if(requestDto.Amount is null)
            throw new ArgumentNullException(nameof(requestDto.Amount));
        Amount = requestDto.Amount.Value;
        if(requestDto.CategoryId is null)
            throw new ArgumentNullException(nameof(requestDto.CategoryId));
        CategoryId = requestDto.CategoryId.Value;
        UserId = userId;
    }
}