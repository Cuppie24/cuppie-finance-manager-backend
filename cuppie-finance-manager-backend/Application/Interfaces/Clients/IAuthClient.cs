using System.Net;
using Application.Dto;
using Application.Dto.Auth;
using Application.Dto.Auth.User;

namespace Application.Interfaces.Clients;

public interface IAuthClient
{
    Task<OperationResult<AuthResponseDto?>> AddUser(AuthRegisterRequestDto requestDto);
    Task<OperationResult<AuthResponseDto?>> Login(AuthLoginRequestDto requestDto);
    Task<OperationResult<AuthResponseDto?>> RefreshTokens(TokenRefreshRequestDto requestDto);
    Task<OperationResult<UserDto?>> ValidateToken(GetCurrentUserDto jwtToken);
    Task<OperationResult<UserDto?>> DeleteUser(int id);
}