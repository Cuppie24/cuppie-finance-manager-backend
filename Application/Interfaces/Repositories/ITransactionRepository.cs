using Application.Dto;

namespace Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<OperationResult<TransactionDto?>> AddTransactionAsync(TransactionDto transaction);
    Task<OperationResult<TransactionDto?>> UpdateTransactionAsync(TransactionDto transaction);
    Task<OperationResult<TransactionDto?>> DeleteTransactionAsync(long id);
    Task<OperationResult<List<TransactionDto>>> GetTransactionsAsync(TransactionFilterDto filter);
    Task<OperationResult<TransactionDto?>> GetTransaction(long id);
}