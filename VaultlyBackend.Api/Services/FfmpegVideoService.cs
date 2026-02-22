using System.Diagnostics;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Services
{
    public class FfmpegVideoService : IFfmpegVideoService
    {
        public async Task ConvertToHls(string inputPath, string outputDir)
        {
            var ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";
            var args = $"-y -i \"{inputPath}\" -c copy -start_number 0 -hls_time 6 -hls_list_size 0 -f hls \"{Path.Combine(outputDir, "index.m3u8")}\"";

            await RunFfmpeg(ffmpegPath, args, outputDir, "HLS");
        }
        public async Task<double> GetVideoDuration(string inputPath)
        {
            var ffprobePath = @"C:\ffmpeg\bin\ffprobe.exe";

            var args = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputPath}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffprobePath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (double.TryParse(output.Trim(), out double duration))
                return duration;

            throw new Exception("Video duration okunamadı.");
        }
        // Thumbnail oluşturma
        public async Task GenerateThumbnail(string inputPath, string outputDir, string thumbnailName = "thumbnail.jpg")
        {
            var ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";

            var thumbnailPath = Path.Combine(outputDir, thumbnailName);

            // 1️⃣ Duration al
            var duration = await GetVideoDuration(inputPath);

            // 2️⃣ %10 hesapla
            var captureSecond = duration * 0.10;

            // Video çok kısaysa 1. saniyeden al
            if (captureSecond < 1)
                captureSecond = 1;

            var args =
                $"-y -i \"{inputPath}\" " +
                "-vf \"thumbnail,scale=1280:-1\" " +
                "-frames:v 1 " +
                "-pix_fmt yuvj420p " +
                $"\"{thumbnailPath}\"";

            await RunFfmpeg(ffmpegPath, args, outputDir, "Thumbnail");

            if (!File.Exists(thumbnailPath))
                throw new Exception("Thumbnail oluşturulamadı.");
        }

        // Ortak FFMPEG çalıştırma
        public async Task RunFfmpeg(string ffmpegPath, string args, string workingDir, string taskName)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = args,
                        WorkingDirectory = workingDir,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                // 🔥 OKUMAYI BAŞLAT
                var stdOutTask = process.StandardOutput.ReadToEndAsync();
                var stdErrTask = process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                var output = await stdOutTask;
                var error = await stdErrTask;

                if (process.ExitCode != 0)
                {
                    //var error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"FFmpeg {taskName} failed: {error}");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
