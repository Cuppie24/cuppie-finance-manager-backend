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
    public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] CreateTransactionDto? transactionDto)
    {
        logger.LogInformation("Transaction post request received:");
        if (transactionDto is null)
        {
            logger.LogWarning("Request body is null");
            return BadRequest("");
        }

        var postResult = await transactionService.AddTransactionAsync(transactionDto);
        logger.LogInformation("Post result: {message}\r\n{result}",postResult.Message, postResult.Message);
        if (postResult.IsSuccess)
            return Ok(postResult.Data);
        return BadRequest(postResult.Message);
    }

    [HttpPatch] 
    public async Task<ActionResult<TransactionDto>> PatchTransaction([FromBody] TransactionDto? transactionDto)
    {
        logger.LogInformation("Transaction patch request received: id: {id}", transactionDto?.Id);
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state");
            return BadRequest(ModelState);
        }
        
        var patchResult = await transactionService.UpdateTransactionAsync(transactionDto);
        if (patchResult.IsSuccess)
            return Ok(patchResult.Data);
        return BadRequest(patchResult.Message);
            
        
    }

    [HttpDelete]
    public async Task<ActionResult<TransactionDto>> DeleteTransaction([FromQuery] long id)
    {
        logger.LogInformation("Transaction delete request received: id: {id}", id);
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state");
            return BadRequest(ModelState);
        }

        var deleteResult = await transactionService.DeleteTransactionAsync(id);
        if (deleteResult.IsSuccess)
            return Ok(deleteResult.Data);
        return BadRequest(deleteResult.Message);
    }
}