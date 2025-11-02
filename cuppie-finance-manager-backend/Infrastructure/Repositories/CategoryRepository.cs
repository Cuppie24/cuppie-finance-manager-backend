using Application.Dto;
using Application.Dto.Finance.CategoryDto;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Entities.Finance;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context, ILogger<CategoryRepository> logger) : ICategoryRepository
{
    public async Task<OperationResult<CategoryDto?>> AddCategory(CategoryEntity categoryToAdd)
    {
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
            categoryToUpdate.Income = newCategory.Income ?? categoryToUpdate.Income;

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

    public async Task<OperationResult<List<CategoryDto>>> GetCategories()
    {
        try
        {
            var query = context.Categories.AsNoTracking();
            var categories = await query.Select(t => new CategoryDto
            {
                Id = t.Id,
                Name = t.Name,
                Income = t.Income
            }).ToListAsync();
            if (categories.Count == 0)
                return OperationResult<List<CategoryDto>>.Failure("Categories not found", OperationStatusCode.NotFound);
            return OperationResult<List<CategoryDto>>.Success(categories);
        }
        catch (Exception ex)
        {
            return OperationResult<List<CategoryDto>>.Failure(ex.Message, OperationStatusCode.InternalError);
        }
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