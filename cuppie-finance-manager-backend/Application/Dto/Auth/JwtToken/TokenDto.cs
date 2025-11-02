namespace Application.Dto.Auth.JwtToken;

public class TokenDto(string token, int expiresInMinutes)
{
    public string Token { get; set; } = token;
    public int ExpiresInMinutes { get; set; } = expiresInMinutes;
}