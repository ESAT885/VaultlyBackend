namespace VaultlyBackend.Api.Services.Interfaces
{
    public interface IFfmpegVideoService
    {
        public  Task ConvertToHls(string inputPath, string outputDir);
        public  Task<double> GetVideoDuration(string inputPath);
        public  Task GenerateThumbnail(string inputPath, string outputDir, string thumbnailName = "thumbnail.jpg");
    }
}
