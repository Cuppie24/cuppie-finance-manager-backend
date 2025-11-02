using Application.Dto;
using Application.Dto.Finance.CategoryDto;

namespace Application.Interfaces.Services;

public interface ICategoryService
{
    public Task<OperationResult<CategoryDto?>> AddCategoryAsync(CreateCategoryDto createCategoryDto);
    public Task<OperationResult<CategoryDto?>> UpdateCategoryAsync(PatchCategoryDto patchCategoryDto);
    public Task<OperationResult<CategoryDto?>> DeleteCategoryAsync(long id);
    public Task<OperationResult<List<CategoryDto>>> GetCategoriesAsync();
    public Task<OperationResult<CategoryDto?>> GetCategoryAsync(long id);
}