namespace VaultlyBackend.Api.Models
{
    public class UserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
