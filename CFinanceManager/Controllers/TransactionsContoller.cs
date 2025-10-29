using Microsoft.AspNetCore.Mvc;
using Application.Dto;
using Application.Interfaces.Services;

namespace CFinanceManager.Controllers;

[Route("api/transactions")]
public class TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult<TransactionDto>> GetTransactions()
    {
        throw new NotImplementedException(); 
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> AddTransaction([FromBody] CreateTransactionDto? transactionDto)
    {
        logger.LogInformation("Post transaction request received:");
        if (transactionDto is null)
        {
            logger.LogWarning("Request body is null");
            return BadRequest();
        }
        logger.LogInformation("Amount: {amount}\r\n" +
                              "Comment: {comment}\r\n", transactionDto?.Amount, transactionDto?.Comment);

        var postResult = await transactionService.AddTransactionAsync(transactionDto);
        logger.LogInformation("Post result: {message}\r\n{result}",postResult.Message, postResult.Message);
        if (postResult.IsSuccess)
            return Ok(postResult.Data);
        return BadRequest(postResult.Message);
    }

    [HttpPatch] 
    public async Task<ActionResult<TransactionDto>> PatchTransaction([FromBody] TransactionDto? transactionDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<ActionResult<TransactionDto>> DeleteTransaction([FromBody] TransactionDto? transactionDto)
    {
        throw new NotImplementedException();
    }
}