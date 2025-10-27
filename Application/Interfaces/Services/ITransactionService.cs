using Application.Dto;

namespace Application.Interfaces.Services;

public interface ITransactionService
{
    public TransactionDto AddTransaction(TransactionDto transaction);
    public TransactionDto UpdateTransaction(TransactionDto transaction);
    public TransactionDto DeleteTransaction(int id);
    public List<TransactionDto> GetTransactions(TransactionFilterDto filter);
}