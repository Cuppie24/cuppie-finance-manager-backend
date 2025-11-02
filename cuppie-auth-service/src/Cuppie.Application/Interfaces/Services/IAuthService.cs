using Cuppie.Application.DTOs;
using Cuppie.Domain.Entities;

namespace Cuppie.Application.Interfaces.Services;

public interface IAuthService
{
    Task<OperationResult<AuthResponseDto>> RegisterAsync(AuthRequestDto authRequest);
    Task<OperationResult<AuthResponseDto>> LoginAsync(AuthRequestDto authRequest);
    Task<OperationResult<AuthResponseDto>> RefreshAccessTokenAsync(GetTokensRequestDto requestDto);
    OperationResult<SafeUserDataDto> GetUserData(string token);
    Task<OperationResult<SafeUserDataDto>> DeleteUserAsync(int id);
}