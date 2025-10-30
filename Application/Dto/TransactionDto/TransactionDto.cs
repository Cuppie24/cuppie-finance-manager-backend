using Domain.Entities;

namespace Application.Dto.TransactionDto;

public class TransactionDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Comment { get; set; }
    public long CategoryId { get; set; }
    public long UserId { get; set; }

    public TransactionDto() { }

    public TransactionDto(int id, decimal amount, DateTime createdAt, string comment)
    {
        Id = id;
        Amount = amount;
        CreatedAt = createdAt;
        Comment = comment;
    }

    public TransactionDto(TransactionEntity entity)
    {
        Id = entity.Id;
        Amount = entity.Amount;
        CreatedAt = entity.CreatedAt;
        Comment = entity.Comment;
        CategoryId = entity.CategoryId;
        UserId = entity.UserId;
    }
}