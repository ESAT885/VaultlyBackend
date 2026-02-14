using VaultlyBackend.Api.Models.Dtos.Auth;
using VaultlyBackend.Api.Models.Dtos.Users;

namespace VaultlyBackend.Api.Services.Interfaces
{
    public interface IAuthService
    {
        public  Task<RegisterUserResponseDto?> RegisterAsync(RegisterUserRequestDto request);
        public Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
        public Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
