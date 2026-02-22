using VaultlyBackend.Api.Models.Entites;

namespace VaultlyBackend.Api.Models.Dtos.StoredFiles
{
    public class StoredFileDto
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string Extension { get; set; }
        public string FilePath { get; set; }
        public FileStatus Status { get; set; }
    }
}
