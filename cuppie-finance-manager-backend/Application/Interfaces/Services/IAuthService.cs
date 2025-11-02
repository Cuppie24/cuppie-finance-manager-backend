using System.Net;
using Application.Dto;
using Application.Dto.Auth;
using Application.Dto.Auth.User;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<OperationResult<AuthResponseDto?>> AddUser(AuthRegisterRequestDto requestDto);
    Task<OperationResult<AuthResponseDto?>> Login(AuthLoginRequestDto requestDto);
    Task<OperationResult<UserDto?>> GetUser(GetCurrentUserDto jwtToken);
    Task<OperationResult<UserDto?>> DeleteUserAsync(int id);
    Task<OperationResult<AuthResponseDto?>> RefreshToken(TokenRefreshRequestDto tokenRequestDto);
}