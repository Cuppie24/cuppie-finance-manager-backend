using System.ComponentModel.DataAnnotations;
using Cuppie.Domain.Entities;

namespace Cuppie.Application.DTOs
{
    public class LoginModelDto
    {
        public LoginModelDto(string username, string password)
        {
            Username = username;
            Password = password;
        }

        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Имя пользователя может содержать только буквы, цифры, дефисы и подчеркивания")]
        public string? Username { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
        [Required]
        public string Ip { get; set; }
    }

    public class RegisterModelDto(string username, string password, string email, string ip)
    {
        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Имя пользователя может содержать только буквы, цифры, дефисы и подчеркивания")]
        public string Username { get; set; } = username;

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Введите корректный email")]
        public string Email { get; set; } = email;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = password;

        [Required] public string Ip { get; set; } = ip;
    }
    
    public class SafeUserDataDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        
        public SafeUserDataDto(){}

        public SafeUserDataDto(UserEntity entity)
        {
            Id =  entity.Id;
            Username = entity.Username;
            Email = entity.Email;
        }
    }
}
