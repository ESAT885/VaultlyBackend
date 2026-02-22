namespace VaultlyBackend.Api.Models.Entites
{
    public class StoredFile
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string? Extension { get; set; }
        public string? FilePath { get; set; }
        public FileStatus? Status { get; set; }
    }

    public enum FileStatus
    {
        Uploading,
        Completed,
        Processing,
        Ready,
        Failed
    }
}
