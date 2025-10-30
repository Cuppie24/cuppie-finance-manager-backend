using Application.Dto.CategoryDto;
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<OperationResult<CategoryDto?>> AddCategory(CategoryEntity categoryToAdd);
    Task<OperationResult<CategoryDto?>> UpdateCategory(PatchCategoryDto newCategory);
    Task<OperationResult<CategoryDto?>> DeleteCategory(long id);
    Task<OperationResult<List<CategoryDto?>>> GetCategories(CategoryFilterDto filter);
    Task<OperationResult<CategoryDto?>> GetCategory(long id);
}