using System.Net;
using Application.Dto.Auth.User;

namespace Application.Dto.Auth;

public class AuthLoginRequestDto(string ip, string username, string password)
{
    public string Ip { get; set; } = ip;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    
    public AuthLoginRequestDto(LoginModelDto loginModel, string ip): this(ip,  loginModel.Username, loginModel.Password){}
}