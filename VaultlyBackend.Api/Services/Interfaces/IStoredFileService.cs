using VaultlyBackend.Api.Models.Dtos.StoredFiles;
using VaultlyBackend.Api.Models.Entites;

namespace VaultlyBackend.Api.Services.Interfaces
{
    public interface IStoredFileService
    {
        public Task<List<StoredFileDto>> GetStoredFiles();
        public  Task<(string fileName, string filePath)> DownLoad(Guid storedFileId);
        public Task<StoredFileDto> FileInit(string fileName);
        public Task UploadChunk(Guid storedFileId, int index, IFormFile file);
        public Task<StoredFileDto> Complete(Guid storedFileId);
    }
}
