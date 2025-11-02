using Application.Dto;
using Application.Dto.Finance.CategoryDto;
using Domain.Entities;
using Domain.Entities.Finance;

namespace Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<OperationResult<CategoryDto?>> AddCategory(CategoryEntity categoryToAdd);
    Task<OperationResult<CategoryDto?>> UpdateCategory(PatchCategoryDto newCategory);
    Task<OperationResult<CategoryDto?>> DeleteCategory(long id);
    Task<OperationResult<List<CategoryDto>>> GetCategories();
    Task<OperationResult<CategoryDto?>> GetCategory(long id);
}