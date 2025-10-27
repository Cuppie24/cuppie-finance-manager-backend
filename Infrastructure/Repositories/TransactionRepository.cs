using Application.Interfaces.Repositories;
using Application.Dto;
using Domain.Entities;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<OperationResult<TransactionDto?>> AddTransaction(TransactionDto transaction)
    {
        var transactionToAdd = new TransactionEntity(transaction.Amount, transaction.CategoryId, transaction.UserId,  transaction.CreatedAt);
        try
        {
            context.Transactions.Add(transactionToAdd);
            await context.SaveChangesAsync();
            return OperationResult<TransactionDto>.Success(new TransactionDto(transactionToAdd));
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<TransactionDto?>> UpdateTransaction(TransactionDto transaction)
    {
        try
        {
            var transactionToUpdate = await context.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);
            if(transactionToUpdate is null) return OperationResult<TransactionDto>.Failure("Transaction not found");
        
            transactionToUpdate.Amount = transaction.Amount;
            transactionToUpdate.CategoryId = transaction.CategoryId;
            transactionToUpdate.UserId = transaction.UserId;
            transactionToUpdate.CreatedAt = transaction.CreatedAt;
            transactionToUpdate.Comment = transaction.Comment;
            
            context.Transactions.Update(transactionToUpdate);
            await context.SaveChangesAsync();
            return OperationResult<TransactionDto>.Success(new TransactionDto(transactionToUpdate));
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<TransactionDto?>> DeleteTransaction(int id)
    {
        try
        {
            var transactionToDelete = await context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            if(transactionToDelete is null) return OperationResult<TransactionDto>.Failure("Transaction not found");

            context.Transactions.Remove(transactionToDelete);
            await context.SaveChangesAsync();
            return OperationResult<TransactionDto?>.Success(new TransactionDto(transactionToDelete));
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<List<TransactionDto>>> GetTransactions(TransactionFilterDto filter)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<TransactionDto?>> GetTransaction(long id)
    {
        try
        {
            var transaction = await context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            if(transaction is null) return OperationResult<TransactionDto?>.Failure("Transaction not found");
            return OperationResult<TransactionDto?>.Success(new TransactionDto(transaction));
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto?>.Failure(ex.Message);
        }
    }
}