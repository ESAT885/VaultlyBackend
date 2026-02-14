namespace VaultlyBackend.Api.Models.Dtos.Auth
{
    public class RegisterUserRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
