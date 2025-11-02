using System.ComponentModel.DataAnnotations;

namespace Cuppie.Application.DTOs;

public class TokenRequest
{
    [Required] public string? JwtToken { get; set; } = string.Empty;
}