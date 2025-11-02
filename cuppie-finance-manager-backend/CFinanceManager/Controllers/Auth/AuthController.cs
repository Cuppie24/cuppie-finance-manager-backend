using System.Net;
using System.Text.Json;
using Application.Dto;
using Application.Dto.Auth;
using Application.Dto.Auth.JwtToken;
using Application.Dto.Auth.User;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CFinanceManager.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private string? GetIp()
    {
        return HttpContext.Connection.RemoteIpAddress.ToString();
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterAsync([FromBody] RegisterModelDto registerModel)
    {
        var operation = "register"; // for logs
        
        var ip = GetIp();
        if (string.IsNullOrWhiteSpace(ip)) return BadRequest("Can't extract id");
        var requestDto = new AuthRegisterRequestDto(registerModel, ip);
        var postResult = await authService.AddUser(requestDto);
        
        var finalResponse = OperationResult<UserDto?>.Failure(postResult.Message,postResult.OperationStatusCode);
        if (postResult.IsSuccess)
        {
            SetTokenCookies(postResult.Data.JwtToken, postResult.Data.RefreshToken);
            var userDto = postResult.Data.User;
            finalResponse = OperationResult<UserDto>.Success(userDto);
        }
        return HandleResponse(finalResponse, operation);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> LoginAsync([FromBody] LoginModelDto loginModel)
    {
        var operation = "login"; // for logs
        var ip = GetIp();
        if (string.IsNullOrWhiteSpace(ip)) return BadRequest("Can't extract id");
        var loginRequest = new AuthLoginRequestDto(loginModel, ip);
        var loginResult = await authService.Login(loginRequest);
        
        var finalResponse = OperationResult<UserDto>.Failure(loginResult.Message,loginResult.OperationStatusCode);
        if (loginResult.IsSuccess)
        {
            SetTokenCookies(loginResult.Data.JwtToken, loginResult.Data.RefreshToken);
            var userDto = loginResult.Data.User;
            finalResponse = OperationResult<UserDto>.Success(userDto);
        }
        return HandleResponse(finalResponse, operation);
    }

    [HttpDelete]
    public async Task<ActionResult<UserDto>> DeleteAsync(int id)
    {
        var operation = "deleting"; // for logs
        var deleteResult = await authService.DeleteUserAsync(id);
        return HandleResponse(deleteResult, operation);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUserAsync()
    {
        var operation = "fetching current user by token"; // for logs
        if (!Request.Cookies.TryGetValue(JwtCookieName, out var jwtToken) || string.IsNullOrWhiteSpace(jwtToken))
            return Unauthorized("JWT cookie not found or empty");
        
        var fetchResult = await authService.GetUser(new GetCurrentUserDto(jwtToken));
        return HandleResponse(fetchResult, operation);
    }

    [HttpGet("refresh")]
    public async Task<ActionResult<UserDto>> RefreshTokenAsync()
    {
        var operation = "refreshing jwt token";
        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized("JWT cookie not found or empty");
        logger.LogInformation("Token refresh request received: {token}", refreshToken); 
        var ip = GetIp();
        if (string.IsNullOrWhiteSpace(ip)) return Unauthorized("Can't extract id");
        var refreshResult = await authService.RefreshToken(new TokenRefreshRequestDto(refreshToken, ip));
        
        var finalResponse = OperationResult<UserDto>.Failure(refreshResult.Message,refreshResult.OperationStatusCode);
        if (refreshResult.IsSuccess)
        {
            SetTokenCookies(refreshResult.Data.JwtToken, refreshResult.Data.RefreshToken);
            var userDto = refreshResult.Data.User;
            finalResponse = OperationResult<UserDto>.Success(userDto);
        }
        return HandleResponse(finalResponse, operation);
    }

    [HttpGet("logout")]
    public ActionResult Logout()
    {
        DeleteCookies();
        return Ok();
    }
    
    private ActionResult HandleResponse<T>(OperationResult<T> result, string operation)
    {
        if (result.IsSuccess)
        {
            logger.LogInformation("{operation} successful: {data}", operation, JsonSerializer.Serialize(result.Data));
            return Ok(result.Data);
        }

        logger.LogError("Error while {operation}: {message}", operation, result.Message);

        return result.OperationStatusCode switch
        {
            OperationStatusCode.NotFound => NotFound(new { message = "User not found" }),
            OperationStatusCode.Conflict => Conflict(new { message = "A conflict occurred while processing the user" }),
            OperationStatusCode.ValidationError => BadRequest(new { message = "Invalid transaction data" }),
            OperationStatusCode.Unauthorized => Unauthorized(new { message = "Unauthorized access" }),
            OperationStatusCode.BadRequest => BadRequest(new { message = "Bad request" }),
            OperationStatusCode.InternalError => StatusCode(500, new { message = "An error occurred while processing your request" }),
            _ => StatusCode(500, new { message = "An unexpected error occurred" })
        };
    }
    
    private const string JwtCookieName = "jwtToken";
    private const string RefreshTokenCookieName = "refreshToken";
    
    private void DeleteCookies()
    {
        Response.Cookies.Delete(JwtCookieName, GetTokenCookieOptions());
        Response.Cookies.Delete(RefreshTokenCookieName, GetTokenCookieOptions());
    }
    
    private void SetTokenCookies(TokenDto accessJwtToken, TokenDto refreshToken)
    {
        Response.Cookies.Append(JwtCookieName, accessJwtToken.Token, GetTokenWriteCookieOptions(accessJwtToken.ExpiresInMinutes));
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken.Token, GetTokenWriteCookieOptions(refreshToken.ExpiresInMinutes));
    }

    private CookieOptions GetTokenWriteCookieOptions(int expiresInMinutes)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes)
        };
    }
    
    private CookieOptions GetTokenCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        };
    }

}