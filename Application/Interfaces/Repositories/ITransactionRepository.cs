using Application.Dto;

namespace Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<OperationResult<TransactionDto?>> AddTransaction(TransactionDto transaction);
    Task<OperationResult<TransactionDto?>> UpdateTransaction(TransactionDto transaction);
    Task<OperationResult<TransactionDto?>> DeleteTransaction(int id);
    Task<OperationResult<List<TransactionDto>>> GetTransactions(TransactionFilterDto filter);
    Task<OperationResult<TransactionDto?>> GetTransaction(long id);
}