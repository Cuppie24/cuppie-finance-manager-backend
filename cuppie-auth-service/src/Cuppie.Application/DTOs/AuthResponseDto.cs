namespace Cuppie.Application.DTOs;

public class AuthResponseDto 
{
    public SafeUserDataDto? User { get; set; } 
    public TokenData? JwtToken { get; set; } 
    public TokenData? RefreshToken { get; set; }
    
    public AuthResponseDto(){}
    
    public AuthResponseDto(SafeUserDataDto user, TokenData jwtToken, TokenData refreshToken)
    {
        User = user;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
    }
}