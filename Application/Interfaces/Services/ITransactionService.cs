using Application.Dto;

namespace Application.Interfaces.Services;

public interface ITransactionService
{
    public Task<OperationResult<TransactionDto?>> AddTransactionAsync(CreateTransactionDto transaction);
    public Task<OperationResult<TransactionDto>> UpdateTransactionAsync(TransactionDto? transaction);
    public Task<OperationResult<TransactionDto>> DeleteTransactionAsync(long id);
    public Task<OperationResult<List<TransactionDto>>> GetTransactionsAsync(TransactionFilterDto filter);
}