namespace VaultlyBackend.Api.Models.Entites
{
    public class Video
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; } = "";
        public string Status { get; set; } = "Uploading";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string StreamUrl { get; set; } = String.Empty;
    }
}
