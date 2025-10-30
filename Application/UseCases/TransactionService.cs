using Application.Interfaces.Services;
using Application.Dto;
using Application.Dto.TransactionDto;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class TransactionService(ITransactionRepository transactionRepository, ILogger<TransactionService> logger): ITransactionService
{
    public async Task<OperationResult<TransactionDto?>> AddTransactionAsync(CreateTransactionDto newTransaction)
    {
        var transactionToAdd = new TransactionDto(newTransaction)
        {
            CreatedAt = DateTime.UtcNow
        };

        var postResult = await transactionRepository.AddTransactionAsync(transactionToAdd);
        return postResult.IsSuccess ? OperationResult<TransactionDto?>.Success(postResult.Data) 
            : OperationResult<TransactionDto>.Failure(postResult.Message, postResult.OperationStatusCode);
    }

    public async Task<OperationResult<TransactionDto?>> UpdateTransactionAsync(PatchTransactionDto transaction)
    {
        var updateResult = await transactionRepository.UpdateTransactionAsync(transaction);
        return updateResult.IsSuccess ? OperationResult<TransactionDto?>.Success(updateResult.Data) 
            : OperationResult<TransactionDto>.Failure(updateResult.Message, updateResult.OperationStatusCode);
    }

    public async Task<OperationResult<TransactionDto?>> DeleteTransactionAsync(long id)
    {
        var deleteResult = await transactionRepository.DeleteTransactionAsync(id);
        if(deleteResult.IsSuccess)
            return OperationResult<TransactionDto?>.Success(deleteResult.Data);
        return OperationResult<TransactionDto>.Failure(deleteResult.Message, deleteResult.OperationStatusCode);
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
        return OperationResult<TransactionDto>.Failure(getResult.Message, getResult.OperationStatusCode);
    }
}