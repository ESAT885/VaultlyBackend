using VaultlyBackend.Api.Models.Dtos.Videos;

namespace VaultlyBackend.Api.Services.Interfaces
{
    public interface IVideoUploadService
    {
        public Task<List<VideoDto>> GetVideos();
        public  Task<VideoDto> VideoInit(string fileName);
        public  Task UploadChunk(Guid videoId, int index, IFormFile file);
        public  Task<VideoDto> Complete(Guid videoId);
        public  Task<(string fullPath, string contentType)> Stream(Guid videoId, string file);
    }
   

}
