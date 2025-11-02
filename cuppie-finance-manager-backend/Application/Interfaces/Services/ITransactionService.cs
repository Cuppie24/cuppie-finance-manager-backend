using Application.Dto;
using Application.Dto.Finance.TransactionDto;

namespace Application.Interfaces.Services;

public interface ITransactionService
{
    public Task<OperationResult<TransactionDto?>> AddTransactionAsync(CreateTransactionDto createTransactionDto);
    public Task<OperationResult<TransactionDto?>> UpdateTransactionAsync(PatchTransactionDto transaction);
    public Task<OperationResult<TransactionDto?>> DeleteTransactionAsync(long id);
    public Task<OperationResult<List<TransactionDto>>> GetTransactionsAsync(TransactionFilterDto filter);
    public Task<OperationResult<TransactionDto?>> GetTransactionAsync(long id);
}