using System.Text.Json;
using Cuppie.Application.DTOs;
using Cuppie.Application.Interfaces.Services;
using Cuppie.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ilogger = Serilog.ILogger;

namespace Cuppie.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService, Ilogger logger, IOptions<JwtOptions> jwtOptions)
        : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterModelDto registerDto)
        {
            var authRequest = new AuthRequestDto()
            {
                Password = registerDto.Password,
                Username = registerDto.Username,
                Email = registerDto.Email,
                Ip = registerDto.Ip
            };

            var result = await authService.RegisterAsync(authRequest);

            if (result.ErrorCode == ErrorCode.Conflict)
                return Conflict("Пользователь с таким именем уже существует");

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return Problem(
                detail: result.ErrorMessage ?? "Неизвестная ошибка при регистрации",
                statusCode: 400,
                title: "Ошибка регистрации"
            );
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginModelDto loginDto)
        {
            var authRequest = new AuthRequestDto()
            {
                Password = loginDto.Password,
                Username = loginDto.Username,
                Ip = loginDto.Ip
            };
            var result = await authService.LoginAsync(authRequest);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            if (result.ErrorCode == ErrorCode.Unauthorized)
                return Unauthorized("Неверный логин или пароль");

            return Problem(
                detail: result.ErrorMessage ?? "Ошибка при логине",
                statusCode: 400,
                title: "Ошибка логина"
            );
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshTokenAsync([FromBody] GetTokensRequestDto requestDto)
        {
            var result = await authService.RefreshAccessTokenAsync(requestDto);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return result.ErrorCode switch
            {
                ErrorCode.Unauthorized => Unauthorized(result.ErrorMessage),
                ErrorCode.NotFound => NotFound(result.ErrorMessage),
                _ => Problem(detail: result.ErrorMessage, statusCode: 400, title: "Ошибка обновления токена")
            };
        }

        [HttpPost("me")]
        public ActionResult<SafeUserDataDto> GetUserData([FromBody] TokenRequest tokenRequest)
        {
            logger.Debug("Get current user data request: body: {body}", JsonSerializer.Serialize(tokenRequest));
            var result = authService.GetUserData(tokenRequest.JwtToken);

            if (result.ErrorCode == ErrorCode.Unauthorized)
                return Unauthorized("Неверные или просроченные jwt токен");

            if (!result.IsSuccess)
                   return Problem(
                    detail: result.ErrorMessage,
                    statusCode: 400,
                    title: "Ошибка получения пользователя"
                );

            return Ok(result.Data);
        }
        
        [HttpDelete]
        public async Task<ActionResult<SafeUserDataDto>> DeleteUser([FromQuery] int id)
        {
            var deleteResult = await authService.DeleteUserAsync(id);
            if (deleteResult.IsSuccess)
                return Ok(deleteResult.Data);
            return Problem(
                detail: deleteResult.ErrorMessage,
                statusCode: 400,
                title: "Ошибка получения пользователя"
            );
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "cuppie-auth-service", timestamp = DateTime.UtcNow });
        }
    }
}

