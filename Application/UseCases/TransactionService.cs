using Application.Interfaces.Services;
using Application.Dto;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class TransactionService(ITransactionRepository transactionRepository, ILogger<TransactionService> logger): ITransactionService
{
    public async Task<OperationResult<TransactionDto?>> AddTransactionAsync(CreateTransactionDto? newTransaction)
    {
        if (newTransaction is null)
            return OperationResult<TransactionDto?>.Failure("Transaction is null");
        var transactionToAdd = new TransactionDto(newTransaction)
        {
            CreatedAt = DateTime.UtcNow
        };

        var postResult = await transactionRepository.AddTransactionAsync(transactionToAdd);
        if (postResult.IsSuccess)
            return OperationResult<TransactionDto?>.Success(postResult.Data);
        return  OperationResult<TransactionDto>.Failure(postResult.Message);
    }

    public async Task<OperationResult<TransactionDto>> UpdateTransactionAsync(TransactionDto transaction)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<TransactionDto>> DeleteTransactionAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<List<TransactionDto>>> GetTransactionsAsync(TransactionFilterDto filter)
    {
        throw new NotImplementedException();
    }
}