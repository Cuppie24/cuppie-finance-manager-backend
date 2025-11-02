namespace Application.Dto.Auth;

public class TokenRefreshRequestDto(string oldRefreshToken, string ip)
{
    public string OldRefreshToken { get; set; } = oldRefreshToken;
    public string Ip { get; set; } = ip;
}