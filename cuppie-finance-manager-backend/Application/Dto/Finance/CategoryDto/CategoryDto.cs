using Domain.Entities.Finance;

namespace Application.Dto.Finance.CategoryDto;

public class CategoryDto
{
    public long Id { get; set; } 
    public string Name { get; set; } 
    public bool? Income { get; set; } 
    public CategoryDto(){}

    public CategoryDto(long id, string name, bool? income)
    {
        Id = id;
        Name = name;
        Income = income;
    }

    public CategoryDto(CategoryEntity categoryEntity) : this (categoryEntity.Id, categoryEntity.Name, categoryEntity.Income) { }
}