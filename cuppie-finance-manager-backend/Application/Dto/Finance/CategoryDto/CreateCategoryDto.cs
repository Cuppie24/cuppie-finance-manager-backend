using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Finance.CategoryDto;

public class CreateCategoryDto
{
    [Required] public string? Name { get; set; }
    [Required] public bool? Income { get; set; }
}