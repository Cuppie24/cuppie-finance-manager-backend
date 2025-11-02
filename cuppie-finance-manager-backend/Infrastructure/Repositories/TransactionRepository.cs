using Application.Dto;
using Application.Dto.Finance.TransactionDto;
using Application.Interfaces.Repositories;
using Domain.Entities.Finance;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<OperationResult<TransactionDto?>> AddTransactionAsync(TransactionEntity transactionToAdd)
    {
        try
        {
            var categoryExists = await context.Categories.AnyAsync(c => c.Id == transactionToAdd.CategoryId);
            if (!categoryExists)
            {
                return OperationResult<TransactionDto?>.Failure(
                    $"Category with Id {transactionToAdd.CategoryId} does not exist",
                    OperationStatusCode.NotFound
                );
            }
            
            context.Transactions.Add(transactionToAdd);
            await context.SaveChangesAsync();
            // Подгрузить навигационное свойство
            await context.Entry(transactionToAdd)
                .Reference(t => t.CategoryEntity)
                .LoadAsync();
            
            return OperationResult<TransactionDto?>.Success(new TransactionDto(transactionToAdd));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
    }

    public async Task<OperationResult<TransactionDto?>> UpdateTransactionAsync(PatchTransactionDto transaction)
    {
        try
        {
            var transactionToUpdate = await context.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);
            if (transactionToUpdate is null)
                return OperationResult<TransactionDto>.Failure("Transaction not found", OperationStatusCode.NotFound);

            transactionToUpdate.Amount = transaction.Amount ?? transactionToUpdate.Amount;
            transactionToUpdate.CategoryId = transaction.CategoryId ?? transactionToUpdate.CategoryId;
            transactionToUpdate.UserId = transaction.UserId ?? transactionToUpdate.UserId;
            transactionToUpdate.CreatedAt = transaction.CreatedAt ?? transactionToUpdate.CreatedAt;
            transactionToUpdate.Comment = transaction.Comment ?? transactionToUpdate.Comment;

            await context.SaveChangesAsync();
            return OperationResult<TransactionDto?>.Success(new TransactionDto(transactionToUpdate));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
    }

    public async Task<OperationResult<TransactionDto?>> DeleteTransactionAsync(long id)
    {
        try
        {
            var transactionToDelete = await context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            if (transactionToDelete is null)
                return OperationResult<TransactionDto>.Failure("Transaction not found", OperationStatusCode.NotFound);

            context.Transactions.Remove(transactionToDelete);
            await context.SaveChangesAsync();
            return OperationResult<TransactionDto?>.Success(new TransactionDto(transactionToDelete));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto>.Failure(ex.Message,  OperationStatusCode.InternalError);
        }
    }

    public async Task<OperationResult<List<TransactionDto>>> GetTransactionsAsync(TransactionFilterDto filter)
    {
        try
        {
            var query = context.Transactions.AsNoTracking();
            
            if (filter.UserId.HasValue)
            {
                query = query.Where(t => t.UserId == filter.UserId);
            }
            
            if (filter.Income.HasValue)
            {
                query = query.Where(t => t.Income == filter.Income.Value);
            }
            
            if (filter.CategoryIdList != null && filter.CategoryIdList.Length > 0)
            {
                query = query.Where(t => filter.CategoryIdList.Contains(t.CategoryId));
            }
            
            if (filter.From.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= filter.From.Value);
            }
            
            if (filter.To.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= filter.To.Value);
            }
            
            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    Amount = t.Amount,
                    Income = t.Income,
                    CreatedAt = t.CreatedAt,
                    Comment = t.Comment
                })
                .ToListAsync();

            return OperationResult<List<TransactionDto>>.Success(transactions);
        }
        catch (Exception ex)
        {
            return OperationResult<List<TransactionDto>>.Failure($"Error: {ex.Message}", OperationStatusCode.InternalError);
        }
    }


    public async Task<OperationResult<TransactionDto?>> GetTransaction(long id)
    {
        try
        {
            var transaction = await context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (transaction is null)
                return OperationResult<TransactionDto?>.Failure("Transaction not found", OperationStatusCode.NotFound);
            return OperationResult<TransactionDto?>.Success(new TransactionDto(transaction));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<TransactionDto?>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto?>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
    }
}