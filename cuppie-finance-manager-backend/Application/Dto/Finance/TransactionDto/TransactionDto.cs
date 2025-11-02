using Domain.Entities.Finance;

namespace Application.Dto.Finance.TransactionDto;

public class TransactionDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public bool Income { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Comment { get; set; }
    public long CategoryId { get; set; }
    public int UserId { get; set; }

    public TransactionDto() { }

    public TransactionDto(int id, decimal amount, DateTime createdAt, string comment, bool income, int userId)
    {
        Id = id;
        Amount = amount;
        CreatedAt = createdAt;
        Comment = comment;
        Income = income;
        UserId = userId;
    }

    public TransactionDto(TransactionEntity entity)
    {
        Id = entity.Id;
        Amount = entity.Amount;
        CreatedAt = entity.CreatedAt;
        Comment = entity.Comment;
        CategoryId = entity.CategoryId;
        UserId = entity.UserId;
        Income = entity.Income;
    }
}