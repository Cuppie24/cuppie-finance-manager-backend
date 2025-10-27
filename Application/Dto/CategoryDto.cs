using Domain.Entities;

namespace Application.Dto;

public class CategoryDto(string? name)
{
    public long Id { get; set; }
    public string? Name { get; set; }

    public CategoryDto(CategoryEntity category) : this(category.Name)
    {
        Id = category.Id;
        Name = category.Name;
    }
}