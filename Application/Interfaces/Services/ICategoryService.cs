using Application.Dto;

namespace Application.Interfaces.Services;

public interface ICategoryService
{
    public CategoryDto AddCategory(CategoryDto categoryDto);
    public CategoryDto UpdateCategory(CategoryDto categoryDto);
    public CategoryDto DeleteCategory(long id);
    public List<CategoryDto> GetCategories(CategoryFilterDto filter);
}