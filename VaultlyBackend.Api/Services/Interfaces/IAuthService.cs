using VaultlyBackend.Api.Entites;
using VaultlyBackend.Api.Models;
using VaultlyBackend.Api.Models.Auth;

namespace VaultlyBackend.Api.Services.Interfaces
{
    public interface IAuthService
    {
        public  Task<RegisterUserResponse?> RegisterAsync(RegisterUserRequest request);
        public Task<TokenResponseDto?> LoginAsync(UserDto request);
        public Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
