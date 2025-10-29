using Application.Dto.CategoryDto;
using Application.Dto;

namespace Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<OperationResult<CategoryDto?>> AddCategory(CategoryDto categoryDto);
    Task<OperationResult<CategoryDto?>> UpdateCategory(CategoryDto newCategory);
    Task<OperationResult<CategoryDto?>> DeleteCategory(long id);
    Task<OperationResult<List<CategoryDto?>>> GetCategories(CategoryFilterDto filter);
    Task<OperationResult<CategoryDto?>> GetCategory(long id);
}