using System.ComponentModel.DataAnnotations;

namespace Cuppie.Application.DTOs;

public class GetTokensRequestDto
{
    public string? OldRefreshToken { get; set; } = null!;
    [Required]
    public string Ip { get; set; } = null!;
}