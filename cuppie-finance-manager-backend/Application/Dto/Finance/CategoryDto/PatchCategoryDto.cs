using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Finance.CategoryDto;

public class PatchCategoryDto
{
    [Required]
    public long? Id { get; set; }
    public string? Name { get; set; }
    public bool? Income { get; set; }
}