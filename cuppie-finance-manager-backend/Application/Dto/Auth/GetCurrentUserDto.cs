using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Auth;

public class GetCurrentUserDto(string jwtToken)
{
    public string? JwtToken { get; set; } = jwtToken;
}