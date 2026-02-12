using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VaultlyBackend.Api.Data;
using VaultlyBackend.Api.Entites;
using VaultlyBackend.Api.Helpers;
using VaultlyBackend.Api.Models;
using VaultlyBackend.Api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using VaultlyBackend.Api.Models.Auth;
namespace VaultlyBackend.Api.Services
{
    public class AuthService(VaultlyDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.PasswordHash) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await CreateTokenResponse(user);

        }
        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            var incomingHash = TokenHasher.Hash(refreshToken);
            if (user.RefreshTokenHash != incomingHash)
            {

                user.RefreshTokenHash = null;
                await context.SaveChangesAsync();
                return null;
            }


            return user;
        }
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokenHash = TokenHasher.Hash(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return refreshToken;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user?.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user?.Role)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
               configuration.GetValue<string>("AppSettings:Token")!
            ));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokendescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(tokendescriptor);
        }

        public async Task<RegisterUserResponse?> RegisterAsync(RegisterUserRequest request)
        {
            if (await context.Users.AnyAsync(u => u.UserName == request.UserName))
            {
                return null;
            }
            var user = new User
            {
                UserName = request.UserName,
                Role = "User"
            };

            user.PasswordHash =
                new PasswordHasher<User>().HashPassword(user, request.Password);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new RegisterUserResponse 
            { 
                UserName= user.UserName,
                Role= user.Role,    
            };
        }
    }
}
