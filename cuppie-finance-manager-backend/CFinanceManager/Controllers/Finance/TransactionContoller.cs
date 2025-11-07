using System.Security.Claims;
using System.Text.Json;
using Application.Dto;
using Application.Dto.Finance.TransactionDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CFinanceManager.Controllers.Finance;

[Route("api/transactions")]
[ApiController]
public class TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TransactionDto>> GetTransaction([FromQuery] long transactionId)
    {
        var operation = "fetching";
        var fetchResult = await transactionService.GetTransactionAsync(transactionId);
        return HandleResponse(fetchResult, operation);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] CreateTransactionRequestDto requestDto)
    {
        int userId = GetUserId();
        if (userId == 0)
            return Unauthorized();
        var createTransactionDto = new CreateTransactionDto(requestDto, userId);
        var operation = "adding";
        var postResult = await transactionService.AddTransactionAsync(createTransactionDto);
        return HandleResponse(postResult, operation);
    }

    [HttpPatch] 
    public async Task<ActionResult<TransactionDto>> PatchTransaction([FromBody] PatchTransactionRequestDto requestDto)
    {
        int? userId = GetUserId();
        if (userId == 0)
            return Unauthorized();
        var patchTransactionDto = new PatchTransactionDto(requestDto, userId);
        var operation = "patching";
        var patchResult = await transactionService.UpdateTransactionAsync(patchTransactionDto);
        return HandleResponse(patchResult, operation);
    }

    [HttpDelete]
    public async Task<ActionResult<TransactionDto>> DeleteTransaction([FromQuery] long transactionId)
    {
        var operation = "deleting";
        var deleteResult = await transactionService.DeleteTransactionAsync(transactionId);
        return HandleResponse(deleteResult, operation);
    }

    [HttpPost("get")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactions([FromBody] TransactionFilterDto filterDto)
    {
        var operation = "getting";
        var userId = GetUserId();
        filterDto.UserId = userId;
        var getResult = await transactionService.GetTransactionsAsync(filterDto);
        return HandleResponse(getResult, operation);
    }
    
    private ActionResult HandleResponse<T>(OperationResult<T> result, string operation)
    {
        if (result.IsSuccess)
        {
            logger.LogInformation("{operation} transaction successful: {data}", operation, JsonSerializer.Serialize(result.Data));
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

    private int GetUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userIdInt = int.Parse(userIdString ?? "0");
        if(userIdInt == 0)
            logger.LogError("Не удалось извлечь UserId из jwt токена");
        return userIdInt;
    }
}