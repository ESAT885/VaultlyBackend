namespace VaultlyBackend.Api.Entites
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string? RefreshTokenHash { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
