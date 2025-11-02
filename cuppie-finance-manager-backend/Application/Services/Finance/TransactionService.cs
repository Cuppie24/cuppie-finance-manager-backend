using Application.Interfaces.Services;
using Application.Dto;
using Application.Dto.Finance.TransactionDto;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Entities.Finance;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class TransactionService(ITransactionRepository transactionRepository, ILogger<TransactionService> logger): ITransactionService
{
    public async Task<OperationResult<TransactionDto?>> AddTransactionAsync(CreateTransactionDto createTransactionDto)
    {
        if(!createTransactionDto.Amount.HasValue)
            return OperationResult<TransactionDto>.Failure("Validation error", OperationStatusCode.ValidationError);
        if(!createTransactionDto.CategoryId.HasValue)
            return OperationResult<TransactionDto>.Failure("Validation error", OperationStatusCode.ValidationError);
        if(!createTransactionDto.UserId.HasValue)
            return OperationResult<TransactionDto>.Failure("Validation error", OperationStatusCode.ValidationError);
        if(!createTransactionDto.Income.HasValue)
            return OperationResult<TransactionDto>.Failure("Validation error", OperationStatusCode.ValidationError);
        
        var transactionToAdd = new TransactionEntity(
            amount: createTransactionDto.Amount.Value, 
            categoryId:createTransactionDto.CategoryId.Value,
            userId:createTransactionDto.UserId.Value,
            income:createTransactionDto.Income.Value,
            createdAt:createTransactionDto.CreatedAt ?? DateTime.UtcNow,
            comment:createTransactionDto.Comment);

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
        var fetchResult = await transactionRepository.GetTransactionsAsync(filter);
        if(fetchResult.IsSuccess)
            return OperationResult<List<TransactionDto>>.Success(fetchResult.Data);
        return OperationResult<List<TransactionDto>>.Failure(fetchResult.Message, fetchResult.OperationStatusCode);
    }

    public async Task<OperationResult<TransactionDto?>> GetTransactionAsync(long id)
    {
        var getResult = await transactionRepository.GetTransaction(id);
        if (getResult.IsSuccess)
            return OperationResult<TransactionDto?>.Success(getResult.Data);
        return OperationResult<TransactionDto>.Failure(getResult.Message, getResult.OperationStatusCode);
    }
}