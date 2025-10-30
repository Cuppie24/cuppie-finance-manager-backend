using System.ComponentModel.DataAnnotations;

namespace Application.Dto.CategoryDto;

public class CreateCategoryDto
{
    [Required] public string? Name { get; set; }
}