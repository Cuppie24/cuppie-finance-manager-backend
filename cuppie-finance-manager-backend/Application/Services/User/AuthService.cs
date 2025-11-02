using System.Net;
using System.Text.Json;
using Application.Dto;
using Application.Dto.Auth;
using Application.Dto.Auth.User;
using Application.Interfaces.Clients;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services.User;

public class AuthService(IAuthClient authClient, ILogger<AuthService> logger) : IAuthService
{
    public async Task<OperationResult<AuthResponseDto?>> AddUser(AuthRegisterRequestDto requestDto)
    {
        var postResult = await authClient.AddUser(requestDto);
        if (!postResult.IsSuccess)
            return OperationResult<AuthResponseDto?>.Failure(postResult.Message, postResult.OperationStatusCode);
        return OperationResult<AuthResponseDto>.Success(postResult.Data);
    }

    public async Task<OperationResult<AuthResponseDto?>> Login(AuthLoginRequestDto requestDto)
    {
        var postResult = await authClient.Login(requestDto);
        if (!postResult.IsSuccess)
            return OperationResult<AuthResponseDto>.Failure(postResult.Message, postResult.OperationStatusCode);
        return OperationResult<AuthResponseDto?>.Success(postResult.Data);
    }

    public async Task<OperationResult<UserDto?>> GetUser(GetCurrentUserDto jwtToken)
    {
        var fetchResult = await authClient.ValidateToken(jwtToken);
        if (!fetchResult.IsSuccess)
            return OperationResult<UserDto>.Failure(fetchResult.Message, fetchResult.OperationStatusCode);
        return OperationResult<UserDto?>.Success(fetchResult.Data);
    }

    public async Task<OperationResult<UserDto?>> DeleteUserAsync(int id)
    {
        var deleteResult = await authClient.DeleteUser(id);
        if (!deleteResult.IsSuccess)
            return OperationResult<UserDto>.Failure(deleteResult.Message, deleteResult.OperationStatusCode);
        return OperationResult<UserDto?>.Success(deleteResult.Data);
    }

    public async Task<OperationResult<AuthResponseDto?>> RefreshToken(TokenRefreshRequestDto tokenRequestDto)
    {
        var refreshResult = await authClient.RefreshTokens(tokenRequestDto);
        if (!refreshResult.IsSuccess)
            return OperationResult<AuthResponseDto>.Failure(refreshResult.Message, refreshResult.OperationStatusCode);
        return OperationResult<AuthResponseDto?>.Success(refreshResult.Data);
    }
}