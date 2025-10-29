using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Application.Dto;
using Application.Dto.TransactionDto;
using Application.Interfaces.Services;

namespace CFinanceManager.Controllers;

[Route("api/transactions")]
public class TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult<TransactionDto>> GetTransaction([FromQuery] long id)
    {
        var getResult = await transactionService.GetTransactionAsync(id);
        if (getResult.IsSuccess)
        {
            logger.LogInformation("Get transaction successful: {result}", JsonSerializer.Serialize(getResult.Data));
            return Ok(getResult.Data);
        }
        logger.LogWarning("Error while fetching transaction: {message}", getResult.Message);
        return BadRequest(new {message = getResult.Message});
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] CreateTransactionDto? transactionDto)
    {
        logger.LogInformation("Transaction post request received:");
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state");
            return BadRequest(ModelState);
        }

        var postResult = await transactionService.AddTransactionAsync(transactionDto);
        if (postResult.IsSuccess)
        {
            logger.LogInformation("Transaction post successful: {result}", JsonSerializer.Serialize(postResult.Data));
            return Ok(postResult.Data);
        }
        logger.LogWarning("Error while posting transaction: {message}", postResult.Message);
        return BadRequest(new {message = postResult.Message});
    }

    [HttpPatch] 
    public async Task<ActionResult<TransactionDto>> PatchTransaction([FromBody] PatchTransactionDto? transactionDto)
    {
        logger.LogInformation("Transaction patch request received: id: {id}", transactionDto?.Id);
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state");
            return BadRequest(ModelState);
        }
        
        var patchResult = await transactionService.UpdateTransactionAsync(transactionDto);
        if (patchResult.IsSuccess)
        {
            logger.LogInformation("Transaction patch successful: {result}", JsonSerializer.Serialize(patchResult.Data));
            return Ok(patchResult.Data);
        }
        logger.LogWarning("Error patching transaction: {message}", patchResult.Message);
        return BadRequest(new {message = patchResult.Message});
            
        
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
        {
            logger.LogInformation("Transaction delete successful: {result}", JsonSerializer.Serialize(deleteResult.Data));
            return Ok(deleteResult.Data);
        }
        logger.LogWarning("Error while deleting transaction: {message}", deleteResult.Message);
        return BadRequest(new {message = deleteResult.Message});
    }
}