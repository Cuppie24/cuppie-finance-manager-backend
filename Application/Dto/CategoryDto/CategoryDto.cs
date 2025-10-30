using Domain.Entities;

namespace Application.Dto.CategoryDto;

public class CategoryDto(long id, string name)
{
    public long Id { get; set; } = id;
    public string Name { get; set; } = name;

    public CategoryDto(CategoryEntity categoryEntity) : this (categoryEntity.Id, categoryEntity.Name) { }
}