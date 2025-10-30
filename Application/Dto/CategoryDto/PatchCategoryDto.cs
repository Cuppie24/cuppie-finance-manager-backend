using System.ComponentModel.DataAnnotations;

namespace Application.Dto.CategoryDto;

public class PatchCategoryDto
{
    [Required]
    public long? Id { get; set; }
    public string? Name { get; set; }
}