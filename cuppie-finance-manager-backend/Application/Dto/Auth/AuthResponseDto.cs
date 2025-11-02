using Application.Dto.Auth.JwtToken;
using Application.Dto.Auth.User;

namespace Application.Dto.Auth;

public class AuthResponseDto
{
    public UserDto? User { get; set; } 
    public TokenDto? JwtToken { get; set; } 
    public TokenDto? RefreshToken { get; set; }
    
    public AuthResponseDto(){}

    public AuthResponseDto(UserDto user, TokenDto jwtToken, TokenDto refreshToken)
    {
        User = user;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
    }
}