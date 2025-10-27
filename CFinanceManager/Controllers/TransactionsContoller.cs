using Microsoft.AspNetCore.Mvc;
using Application.Dto;

namespace CFinanceManager.Controllers;

[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    [HttpGet("get")]
    public async Task<ActionResult<TransactionDto>> GetTransactions()
    {
        throw new NotImplementedException(); 
    }

    [HttpPost("post")]
    public async Task<ActionResult<TransactionDto>> PostTransaction(TransactionDto transactionDto)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("patch")] 
    public async Task<ActionResult<TransactionDto>> PatchTransaction(TransactionDto transactionDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<TransactionDto>> DeleteTransaction(TransactionDto transactionDto)
    {
        throw new NotImplementedException();
    }
}