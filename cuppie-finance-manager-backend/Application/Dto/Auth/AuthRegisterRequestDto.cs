using System.Net;
using Application.Dto.Auth.User;

namespace Application.Dto.Auth;

public class AuthRegisterRequestDto(string ip, string username, string password, string email)
{
    public string Ip { get; set; } = ip;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public string Email { get; set; } = email;

    public AuthRegisterRequestDto(RegisterModelDto registerModelDto, string ip) : this(
        ip,
        registerModelDto.Username,
        registerModelDto.Password,
        registerModelDto.Email)
    {
    }
}