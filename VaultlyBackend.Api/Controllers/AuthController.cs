using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VaultlyBackend.Api.Exceptions;
using VaultlyBackend.Api.Models;
using VaultlyBackend.Api.Models.Auth;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : BaseController
    {

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                throw new BusinessException("Invalid  refresh token", "Invalid  refresh token");
            return Success<TokenResponseDto?>(result);
        }
        [EnableRateLimiting("login")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
                throw new BusinessException("Invalid  username or password", "Invalid  username or password");

            return Success<TokenResponseDto>(token);

        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {


            var registerUserResponse = await authService.RegisterAsync(request);
            if (registerUserResponse is null)
            {
                throw new BusinessException("", "BadRequest");

            }
            return Success(registerUserResponse);
        }
        [Authorize]
        [HttpGet("AuthenticateOnlyEndpoint")]
        public IActionResult AuthenticateOnlyEndpoint()
        {
            var userName = User?.Identity?.Name;
            return Success(userName);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AdminOlyEndpoint")]
        public IActionResult AdminOlyEndpoint()
        {
            var userName = User?.Identity?.Name;
            return Success<string?>(userName);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id == 1)
                throw new BusinessException("", "Admin silinemez");

            return Success("Silindi");
        }

    }
}
