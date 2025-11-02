using System.Text.Json;
using Application.Dto;
using Application.Dto.Finance.CategoryDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CFinanceManager.Controllers.Finance;

[Route("api/categories")]
[ApiController]
public class CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CategoryDto>> GetCategory([FromQuery] long id)
    {
        var operation = "fetching"; // for logs
        var fetchResult = await categoryService.GetCategoryAsync(id);
        return HandleResponse(fetchResult, operation);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var operation = "getting";
        var fetchResult = await categoryService.GetCategoriesAsync();
        return HandleResponse(fetchResult, operation);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> PostCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var operation = "adding"; // for logs
        var postResult = await categoryService.AddCategoryAsync(categoryDto);
        return HandleResponse(postResult, operation);
    }

    [HttpDelete]
    public async Task<ActionResult<CategoryDto>> DeleteCategoryAsync(long id)
    {
        if(id == 1)
            return BadRequest("You can't delete this category");
        var operation = "deleting"; // for logs
        var deleteResult = await categoryService.DeleteCategoryAsync(id);
        return HandleResponse(deleteResult, operation);
    }

    [HttpPatch]
    public async Task<ActionResult<CategoryDto>> UpdateCategoryAsync(PatchCategoryDto patchCategoryDto)
    {
        var operation = "updating"; // for logs
        var patchResult = await categoryService.UpdateCategoryAsync(patchCategoryDto);
        return HandleResponse(patchResult, operation);
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
            OperationStatusCode.NotFound => NotFound(new { message = "Category not found" }),
            OperationStatusCode.Conflict => Conflict(new { message = "A conflict occurred while processing the category" }),
            OperationStatusCode.ValidationError => BadRequest(new { message = "Invalid category data" }),
            OperationStatusCode.Unauthorized => Unauthorized(new { message = "Unauthorized access" }),
            OperationStatusCode.BadRequest => BadRequest(new { message = "Bad request" }),
            OperationStatusCode.InternalError => StatusCode(500, new { message = "An error occurred while processing your request" }),
            _ => StatusCode(500, new { message = "An unexpected error occurred" })
        };
    }
}