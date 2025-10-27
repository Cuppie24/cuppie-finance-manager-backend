using Application.Interfaces.Services;
using Application.Dto;

namespace Application.UseCases;

public class TransactionService: ITransactionService
{
    public TransactionDto AddTransaction(TransactionDto transaction)
    {
        throw new NotImplementedException();
    }

    public TransactionDto UpdateTransaction(TransactionDto transaction)
    {
        throw new NotImplementedException();
    }

    public TransactionDto DeleteTransaction(int id)
    {
        throw new NotImplementedException();
    }

    public List<TransactionDto> GetTransactions(TransactionFilterDto filter)
    {
        throw new NotImplementedException();
    }
}