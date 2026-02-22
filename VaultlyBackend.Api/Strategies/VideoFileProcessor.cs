using VaultlyBackend.Api.Background;
using VaultlyBackend.Api.Services.Interfaces;
using VaultlyBackend.Api.Strategies.Interfaces;

namespace VaultlyBackend.Api.Strategies
{
    public class VideoFileProcessor(IBackgroundTaskQueue taskQueue, IWebHostEnvironment _env, IFfmpegVideoService ffmpegVideoService) : IFileProcessor
    {
        public bool CanProcess(string contentType)
            => contentType.StartsWith("video/");

        public async Task ProcessAsync(string mergedPath, string storedFilefileName)
        {
            var hlsDir = Path.Combine(
             _env.ContentRootPath,
             "storage",
             "hls",
             storedFilefileName
         );
            Directory.CreateDirectory(hlsDir);
            await taskQueue.QueueAsync(new BackgroundJob
            {
                Name = $"HLS Dönüştürme - VideoId: " + storedFilefileName,
                WorkItem = async token =>
                {
                    await ffmpegVideoService.ConvertToHls(mergedPath, hlsDir);
                }
            });
            await taskQueue.QueueAsync(new BackgroundJob
            {
                Name = $"Thumbnail Oluşturma - VideoId: " + storedFilefileName,
                WorkItem = async token =>
                {
                    await ffmpegVideoService.GenerateThumbnail(mergedPath, hlsDir);
                }
            });
        }
    }
}
