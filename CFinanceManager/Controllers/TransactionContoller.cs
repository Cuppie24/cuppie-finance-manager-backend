using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Application.Dto;
using Application.Dto.TransactionDto;
using Application.Interfaces.Services;

namespace CFinanceManager.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult<TransactionDto>> GetTransaction([FromQuery] long id)
    {
        var operation = "fetching";
        var getResult = await transactionService.GetTransactionAsync(id);
        if (getResult.IsSuccess)
            return HandleSuccessResponse(getResult, operation);
        return HandleErrorResponse(getResult, operation);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] CreateTransactionDto transactionDto)
    {
        var operation = "adding";
        var postResult = await transactionService.AddTransactionAsync(transactionDto);
        if (postResult.IsSuccess)
            return HandleSuccessResponse(postResult, operation);
        return HandleErrorResponse(postResult, operation);
    }

    [HttpPatch] 
    public async Task<ActionResult<TransactionDto>> PatchTransaction([FromBody] PatchTransactionDto transactionDto)
    {
        var operation = "patching";
        var patchResult = await transactionService.UpdateTransactionAsync(transactionDto);
        if (patchResult.IsSuccess)
            return HandleSuccessResponse(patchResult, operation);
        return HandleErrorResponse(patchResult, operation);
    }

    [HttpDelete]
    public async Task<ActionResult<TransactionDto>> DeleteTransaction([FromQuery] long id)
    {
        var operation = "deleting";
        var deleteResult = await transactionService.DeleteTransactionAsync(id);
        if (deleteResult.IsSuccess)
            return HandleSuccessResponse(deleteResult, operation);
        return HandleErrorResponse(deleteResult, operation);
    }


    private ActionResult HandleSuccessResponse<T>(OperationResult<T> result, string operation)
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException("HandleSuccessResponse called for a fail result");
        logger.LogInformation("{operation} successful: {data}", operation, JsonSerializer.Serialize(result.Data));
        return Ok(result.Data);
    }
    private ActionResult HandleErrorResponse<T>(OperationResult<T> result, string operation)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("HandleErrorResponse called for a successful result");
        logger.LogError("Error while {operation} transaction: {message}", operation, result.Message);

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