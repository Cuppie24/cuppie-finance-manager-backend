using Application.Dto;
using Application.Interfaces.Repositories;
using Infrastructure.DbContext;
using Application.Dto.CategoryDto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<OperationResult<CategoryDto?>> AddCategory(CreateCategoryDto categoryDto)
    {
        var categoryToAdd = new CategoryEntity(categoryDto.Name);
        try
        {
            context.Categories.Add(categoryToAdd);
            await context.SaveChangesAsync();
            return OperationResult<CategoryDto?>.Success(new CategoryDto(categoryToAdd));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
    }

    public async Task<OperationResult<CategoryDto?>> UpdateCategory(PatchCategoryDto newCategory)
    {
        try
        {
            var categoryToUpdate =
                await context.Categories.FirstOrDefaultAsync(c => c.Id == newCategory.Id);
            if (categoryToUpdate is null)
                return OperationResult<CategoryDto?>.Failure("Category not found", OperationStatusCode.NotFound);

            categoryToUpdate.Name = newCategory.Name ?? categoryToUpdate.Name;

            await context.SaveChangesAsync();
            return OperationResult<CategoryDto?>.Success(new CategoryDto(categoryToUpdate));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
    }

    public async Task<OperationResult<CategoryDto?>> DeleteCategory(long id)
    {
        try
        {
            var categoryToDelete = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoryToDelete is null)
                return OperationResult<CategoryDto?>.Failure("Category not found", OperationStatusCode.NotFound);

            context.Categories.Remove(categoryToDelete);
            await context.SaveChangesAsync();
            return OperationResult<CategoryDto?>.Success(new CategoryDto(categoryToDelete));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
    }

    public async Task<OperationResult<List<CategoryDto?>>> GetCategories(CategoryFilterDto filter)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<CategoryDto?>> GetCategory(long id)
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return category is null
                ? OperationResult<CategoryDto>.Failure("Category not found", OperationStatusCode.NotFound)
                : OperationResult<CategoryDto?>.Success(new CategoryDto(category));
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
    }
}