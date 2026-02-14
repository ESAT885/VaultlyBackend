namespace VaultlyBackend.Api.Models.Dtos.Videos
{
    public class VideoDto
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; } = "";
        public string Status { get; set; } = "Uploading";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ThumbnailUrl { get; set; }=String.Empty;
    }
}
