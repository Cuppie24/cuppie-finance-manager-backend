using System.Text.Json;
using Application.Dto;
using Application.Dto.CategoryDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CFinanceManager.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategory([FromQuery] long id)
    {
        var operation = "fetching"; // for logs
        var fetchResult = await categoryService.GetCategoryAsync(id);
        if (fetchResult.IsSuccess)
            return HandleSuccessResponse(fetchResult, operation);
        return HandleErrorResponse(fetchResult, operation);
    }

    [HttpPost]
    public async Task<IActionResult> PostCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var operation = "adding"; // for logs
        var postResult = await categoryService.AddCategoryAsync(categoryDto);
        if (postResult.IsSuccess) 
            return HandleSuccessResponse(postResult, operation);
        return HandleErrorResponse(postResult, operation);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCategoryAsync(long id)
    {
        var operation = "deleting"; // for logs
        var deleteResult = await categoryService.DeleteCategoryAsync(id);
        if (deleteResult.IsSuccess)
            return HandleSuccessResponse(deleteResult, operation);
        return HandleErrorResponse(deleteResult, operation);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateCategoryAsync(PatchCategoryDto patchCategoryDto)
    {
        var operation = "updating";
        var patchResult = await categoryService.UpdateCategoryAsync(patchCategoryDto);
        if (patchResult.IsSuccess)
            return HandleSuccessResponse(patchResult, operation);
        return HandleErrorResponse(patchResult, operation);
    }

    private ActionResult HandleSuccessResponse<T>(OperationResult<T> result, string operation)
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException("HandleSuccessResponse called for a fail result");
        logger.LogInformation("{operation} category successful: {data}", operation, JsonSerializer.Serialize(result.Data));
        return Ok(result.Data);
    }
    private ActionResult HandleErrorResponse<T>(OperationResult<T> result, string operation)
    {
        if(result.IsSuccess)
            throw new InvalidOperationException("HandleErrorResponse called for a successful result");
        logger.LogError("error while {operation} category: {message}", operation, result.Message);

        return result.OperationStatusCode switch
        {
            OperationStatusCode.NotFound => NotFound(new { message = "Category not found" }),
            OperationStatusCode.Conflict => Conflict(new
                { message = "A conflict occurred while processing the category" }),
            OperationStatusCode.ValidationError => BadRequest(new { message = "Invalid category data" }),
            OperationStatusCode.InternalError => StatusCode(500,
                new { message = "An error occurred while processing your request" }),
            _ => StatusCode(500, new { message = "An unexpected error occurred" })
        };
    }
}