namespace VaultlyBackend.Api.Models.Dtos.Auth
{
    public class RegisterUserResponseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
