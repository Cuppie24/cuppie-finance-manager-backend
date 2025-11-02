using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Auth.User;

public class RegisterModelDto
{
    [Required]
    [StringLength(30, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z0-9_-]+$", ErrorMessage = "Имя пользователя может содержать только буквы, цифры, дефисы и подчеркивания")]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}