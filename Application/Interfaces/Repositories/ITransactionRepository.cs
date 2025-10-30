﻿using Application.Dto;
using Application.Dto.TransactionDto;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<OperationResult<TransactionDto?>> AddTransactionAsync(TransactionEntity transactionToAdd);
    Task<OperationResult<TransactionDto?>> UpdateTransactionAsync(PatchTransactionDto transaction);
    Task<OperationResult<TransactionDto?>> DeleteTransactionAsync(long id);
    Task<OperationResult<List<TransactionDto>>> GetTransactionsAsync(TransactionFilterDto filter);
    Task<OperationResult<TransactionDto?>> GetTransaction(long id);
}