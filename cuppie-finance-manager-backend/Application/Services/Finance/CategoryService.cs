using Application.Dto;
using Application.Dto.Finance.CategoryDto;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Entities.Finance;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<OperationResult<CategoryDto?>> AddCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        if (string.IsNullOrWhiteSpace(createCategoryDto.Name))
            return OperationResult<CategoryDto?>.Failure("Invalid createCategoryDto", OperationStatusCode.BadRequest);
        
        var categoryEntity = new CategoryEntity(createCategoryDto.Name, createCategoryDto.Income);
        var postResult = await categoryRepository.AddCategory(categoryEntity);
        if (postResult.IsSuccess)
            return OperationResult<CategoryDto?>.Success(postResult.Data);
        return OperationResult<CategoryDto?>.Failure(postResult.Message, postResult.OperationStatusCode);
    }

    public async Task<OperationResult<CategoryDto?>> UpdateCategoryAsync(PatchCategoryDto patchCategoryDto)
    {
        var updateResult = await categoryRepository.UpdateCategory(patchCategoryDto);
        if (updateResult.IsSuccess)
            return OperationResult<CategoryDto?>.Success(updateResult.Data);
        return OperationResult<CategoryDto?>.Failure(updateResult.Message, updateResult.OperationStatusCode);
    }

    public async Task<OperationResult<CategoryDto?>> DeleteCategoryAsync(long id)
    {
        var deleteResult = await categoryRepository.DeleteCategory(id);
        if (deleteResult.IsSuccess)
            return OperationResult<CategoryDto?>.Success(deleteResult.Data);
        return OperationResult<CategoryDto?>.Failure(deleteResult.Message, deleteResult.OperationStatusCode);
    }

    public async Task<OperationResult<List<CategoryDto>>> GetCategoriesAsync()
    {
        var fetchResult = await categoryRepository.GetCategories();
        if (fetchResult.IsSuccess)
            return OperationResult<List<CategoryDto>>.Success(fetchResult.Data);
        return OperationResult<List<CategoryDto>>.Failure(fetchResult.Message, fetchResult.OperationStatusCode);
    }

    public async Task<OperationResult<CategoryDto?>> GetCategoryAsync(long id)
    {
        var fetchResult = await categoryRepository.GetCategory(id);
        if (fetchResult.IsSuccess)
            return OperationResult<CategoryDto?>.Success(fetchResult.Data);
        return OperationResult<CategoryDto?>.Failure(fetchResult.Message, fetchResult.OperationStatusCode);
    }
}