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
        return postResult.IsSuccess ? OperationResult<TransactionDto?>.Success(postResult.Data) 
            : OperationResult<TransactionDto>.Failure(postResult.Message);
    }

    public async Task<OperationResult<TransactionDto?>> UpdateTransactionAsync(TransactionDto? transaction)
    {
        if(transaction is null)
            return OperationResult<TransactionDto>.Failure("Transaction is null");
        var updateResult = await transactionRepository.UpdateTransactionAsync(transaction);
        return updateResult.IsSuccess ? OperationResult<TransactionDto?>.Success(updateResult.Data) 
            : OperationResult<TransactionDto>.Failure(updateResult.Message);
    }

    public async Task<OperationResult<TransactionDto?>> DeleteTransactionAsync(long id)
    {
        var deleteResult = await transactionRepository.DeleteTransactionAsync(id);
        if(deleteResult.IsSuccess)
            return OperationResult<TransactionDto?>.Success(deleteResult.Data);
        return OperationResult<TransactionDto>.Failure(deleteResult.Message);
    }

    public async Task<OperationResult<List<TransactionDto>>> GetTransactionsAsync(TransactionFilterDto filter)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<TransactionDto?>> GetTransactionAsync(long id)
    {
        var getResult = await transactionRepository.GetTransaction(id);
        if (getResult.IsSuccess)
            return OperationResult<TransactionDto?>.Success(getResult.Data);
        return OperationResult<TransactionDto>.Failure(getResult.Message);
    }
}