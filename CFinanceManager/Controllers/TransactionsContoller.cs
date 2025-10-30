using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Application.Dto;
using Application.Dto.TransactionDto;
using Application.Interfaces.Services;

namespace CFinanceManager.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult<TransactionDto>> GetTransaction([FromQuery] long id)
    {
        logger.LogInformation("Transaction get request received: id: {id}", id);
        var getResult = await transactionService.GetTransactionAsync(id);
        
        if (getResult.IsSuccess)
        {
            logger.LogInformation("Get transaction successful: {result}", JsonSerializer.Serialize(getResult.Data));
            return Ok(getResult.Data);
        }
        
        return HandleErrorResult(getResult, "fetching");
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] CreateTransactionDto? transactionDto)
    {
        logger.LogInformation("Transaction post request received");
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state");
            return BadRequest(ModelState);
        }

        var postResult = await transactionService.AddTransactionAsync(transactionDto);
        
        if (postResult.IsSuccess)
        {
            logger.LogInformation("Transaction post successful: {result}", JsonSerializer.Serialize(postResult.Data));
            return CreatedAtAction(nameof(GetTransaction), new { id = postResult.Data?.Id }, postResult.Data);
        }
        
        return HandleErrorResult(postResult, "creating");
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
        
        return HandleErrorResult(patchResult, "updating");
    }

    [HttpDelete]
    public async Task<ActionResult<TransactionDto>> DeleteTransaction([FromQuery] long id)
    {
        logger.LogInformation("Transaction delete request received: id: {id}", id);

        var deleteResult = await transactionService.DeleteTransactionAsync(id);
        
        if (deleteResult.IsSuccess)
        {
            logger.LogInformation("Transaction delete successful: {result}", JsonSerializer.Serialize(deleteResult.Data));
            return Ok(deleteResult.Data);
        }
        
        return HandleErrorResult(deleteResult, "deleting");
    }

    private ActionResult HandleErrorResult<T>(OperationResult<T> result, string operation)
    {
        // Логируем детальную ошибку для внутреннего использования
        logger.LogError("Error while {operation} transaction: {message}", operation, result.Message);

        // Возвращаем клиенту общее сообщение без деталей
        return result.OperationStatusCode switch
        {
            OperationStatusCode.NotFound => NotFound(new { message = "Transaction not found" }),
            OperationStatusCode.Conflict => Conflict(new { message = "A conflict occurred while processing the transaction" }),
            OperationStatusCode.ValidationError => BadRequest(new { message = "Invalid transaction data" }),
            OperationStatusCode.InternalError => StatusCode(500, new { message = "An error occurred while processing your request" }),
            _ => StatusCode(500, new { message = "An unexpected error occurred" })
        };
    }
}