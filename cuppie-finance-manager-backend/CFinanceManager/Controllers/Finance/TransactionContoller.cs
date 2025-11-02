using System.Security.Claims;
using System.Text.Json;
using Application.Dto;
using Application.Dto.Finance.TransactionDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CFinanceManager.Controllers.Finance;

[Route("api/transactions")]
[ApiController]
public class TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TransactionDto>> GetTransaction([FromQuery] long id)
    {
        var operation = "fetching";
        var fetchResult = await transactionService.GetTransactionAsync(id);
        return HandleResponse(fetchResult, operation);
    }

    [HttpPost]
    // [Authorize]
    public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] CreateTransactionDto transactionDto)
    {
        // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        // if (userIdClaim is null) return Unauthorized();
        // logger.LogError($"User with id: {userIdClaim?.Value}");
        // if (transactionDto.UserId != long.Parse(userIdClaim.Value))
        //     return Unauthorized();
        var operation = "adding";
        var postResult = await transactionService.AddTransactionAsync(transactionDto);
        return HandleResponse(postResult, operation);
    }

    [HttpPatch] 
    public async Task<ActionResult<TransactionDto>> PatchTransaction([FromBody] PatchTransactionDto transactionDto)
    {
        var operation = "patching";
        var patchResult = await transactionService.UpdateTransactionAsync(transactionDto);
        return HandleResponse(patchResult, operation);
    }

    [HttpDelete]
    public async Task<ActionResult<TransactionDto>> DeleteTransaction([FromQuery] long id)
    {
        var operation = "deleting";
        var deleteResult = await transactionService.DeleteTransactionAsync(id);
        return HandleResponse(deleteResult, operation);
    }

    [HttpPost("get")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactions([FromBody] TransactionFilterDto filterDto)
    {
        var operation = "getting";
        var getResult = await transactionService.GetTransactionsAsync(filterDto);
        return HandleResponse(getResult, operation);
    }
    
    private ActionResult HandleResponse<T>(OperationResult<T> result, string operation)
    {
        if (result.IsSuccess)
        {
            logger.LogInformation("{operation} successful: {data}", operation, JsonSerializer.Serialize(result.Data));
            return Ok(result.Data);
        }

        logger.LogError("Error while {operation}: {message}", operation, result.Message);

        return result.OperationStatusCode switch
        {
            OperationStatusCode.NotFound => NotFound(new { message = "Transaction not found" }),
            OperationStatusCode.Conflict => Conflict(new { message = "A conflict occurred while processing the transaction" }),
            OperationStatusCode.ValidationError => BadRequest(new { message = "Invalid transaction data" }),
            OperationStatusCode.Unauthorized => Unauthorized(new { message = "Unauthorized access" }),
            OperationStatusCode.BadRequest => BadRequest(new { message = "Bad request" }),
            OperationStatusCode.InternalError => StatusCode(500, new { message = "An error occurred while processing your request" }),
            _ => StatusCode(500, new { message = "An unexpected error occurred" })
        };
    }
}