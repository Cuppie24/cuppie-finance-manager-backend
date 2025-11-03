using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Dto;
using Application.Dto.Auth;
using Application.Dto.Auth.User;
using Application.Interfaces.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Clients;

public class AuthClient(HttpClient httpClient, ILogger<AuthClient> logger, IConfiguration configuration) : IAuthClient
{
    private string BaseUrl { get; } = configuration["AuthService:BaseUrl"] ?? "http://localhost:5001";

    public async Task<OperationResult<AuthResponseDto?>> AddUser(AuthRegisterRequestDto requestDto)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/api/auth/register", requestDto);
        if (!response.IsSuccessStatusCode)
            return await FailureAsync<AuthResponseDto>(response);

        return await ParseResponse<AuthResponseDto>(response);
    }

    public async Task<OperationResult<AuthResponseDto?>> Login(AuthLoginRequestDto requestDto)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/api/auth/login", requestDto);
        if (!response.IsSuccessStatusCode)
            return await FailureAsync<AuthResponseDto>(response);

        return await ParseResponse<AuthResponseDto>(response);
    }

    public async Task<OperationResult<AuthResponseDto?>> RefreshTokens(TokenRefreshRequestDto requestDto)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/api/auth/refresh", requestDto);
        if (!response.IsSuccessStatusCode)
            return await FailureAsync<AuthResponseDto>(response);

        return await ParseResponse<AuthResponseDto>(response);
    }

    public async Task<OperationResult<UserDto?>> ValidateToken(GetCurrentUserDto jwtToken)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/api/auth/me", jwtToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogDebug("Error while validating token. Auth service response: {body}", response.Content.ReadAsStringAsync().Result);
            return await FailureAsync<UserDto>(response);
        }


        return await ParseResponse<UserDto>(response);
    }

    public async Task<OperationResult<UserDto?>> DeleteUser(int id)
    {
        var response = await httpClient.DeleteAsync($"{BaseUrl}/api/auth?id={id}");
        if (!response.IsSuccessStatusCode)
            return await FailureAsync<UserDto>(response);

        return await ParseResponse<UserDto>(response);
    }
    
    private async Task<OperationResult<T?>> ParseResponse<T>(HttpResponseMessage response)
    {
        try
        {
            var result = await response.Content.ReadFromJsonAsync<T>();

            if (result is null)
            {
                var body = await response.Content.ReadAsStringAsync();
                logger.LogDebug("Auth service returned empty or invalid JSON. Body: {body}", body);
                logger.LogError("Auth service returned empty or invalid JSON");
                return HandleEmptyOrInvalidResponse<T>();
            }
            logger.LogDebug("Successfully parsed response from auth service to {type}: {response}", typeof(T).Name, JsonSerializer.Serialize(result));
            return OperationResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            var body = await response.Content.ReadAsStringAsync();
            logger.LogDebug(ex, "Error parsing {TypeName} from auth service. Raw body: {body}", typeof(T).Name, body);
            logger.LogError("Error parsing {TypeName} from auth service", typeof(T).Name);
            return HandleEmptyOrInvalidResponse<T>();
        }
    }

    private static OperationStatusCode MapStatusCode(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.OK => OperationStatusCode.Ok,
        HttpStatusCode.BadRequest => OperationStatusCode.BadRequest,
        HttpStatusCode.NotFound => OperationStatusCode.NotFound,
        HttpStatusCode.Unauthorized => OperationStatusCode.Unauthorized,
        HttpStatusCode.Conflict => OperationStatusCode.Conflict,
        HttpStatusCode.UnprocessableEntity => OperationStatusCode.ValidationError,
        _ => OperationStatusCode.InternalError
    };

    private OperationResult<T?> HandleEmptyOrInvalidResponse<T>()
    {
        logger.LogInformation("Auth service returned empty or invalid response body");
        return OperationResult<T>.Failure(
            "Auth service returned empty or invalid response body",
            OperationStatusCode.InternalError);
    }

    private async Task<OperationResult<T?>> FailureAsync<T>(HttpResponseMessage response)
    {
        var statusCode = MapStatusCode(response.StatusCode);
        string errorMessage;

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            logger.LogDebug("Auth service error response body: {body}", body);

            // Попытка извлечь сообщение из JSON ответа
            if (!string.IsNullOrWhiteSpace(body))
            {
                // Сервис возвращает просто строку в теле ответа для Conflict и Unauthorized
                if (response.StatusCode == HttpStatusCode.Conflict ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    errorMessage = body.Trim('"'); // Убираем кавычки если есть
                }
                else
                {
                    errorMessage = body;
                }
            }
            else
            {
                errorMessage = $"Auth service error: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading error response from auth service");
            errorMessage = $"Auth service error: {response.StatusCode}";
        }

        return OperationResult<T>.Failure(errorMessage, statusCode);
    }
}