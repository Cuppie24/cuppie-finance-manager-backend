using Domain.Entities;

namespace Application.Dto;

public class TransactionDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Comment { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }

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