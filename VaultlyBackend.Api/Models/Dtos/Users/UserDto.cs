namespace VaultlyBackend.Api.Models.Dtos.Users
{
    public class UserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
