using System.Runtime.CompilerServices;
using Application.Interfaces.Repositories;
using Infrastructure.DbContext;
using Application.Dto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<OperationResult<CategoryDto?>> AddCategory(CategoryDto categoryDto)
    {
        var categoryToAdd = new CategoryEntity(categoryDto.Name);
        try
        {
            context.Add(categoryToAdd);
            await context.SaveChangesAsync();
            return OperationResult<CategoryDto?>.Success(new CategoryDto(categoryToAdd));
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<CategoryDto?>> UpdateCategory(CategoryDto newCategory)
    {
        try
        {
            var categoryToUpdate = 
                await context.Categories.FirstOrDefaultAsync(c => c.Id == newCategory.Id);
            if (categoryToUpdate is null)
                return OperationResult<CategoryDto?>.Failure("Category not found");
        
            categoryToUpdate.Name = newCategory.Name;
            
            context.Categories.Update(categoryToUpdate);
            await context.SaveChangesAsync();
            return OperationResult<CategoryDto?>.Success(new CategoryDto(categoryToUpdate));
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<CategoryDto?>> DeleteCategory(long id)
    {
        try
        {
            var categoryToDelete = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoryToDelete is null)
                return OperationResult<CategoryDto?>.Failure("Category not found");
            
            context.Categories.Remove(categoryToDelete);
            await context.SaveChangesAsync();
            return OperationResult<CategoryDto?>.Success(new CategoryDto(categoryToDelete));
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message);
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
            if (category is null)
                return OperationResult<CategoryDto>.Failure("Category not found");
            return OperationResult<CategoryDto?>.Success(new CategoryDto(category));
        }
        catch (Exception ex)
        {
            return OperationResult<CategoryDto?>.Failure(ex.Message);
        }
    }
}