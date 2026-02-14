using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VaultlyBackend.Api.Background;
using VaultlyBackend.Api.Data;
using VaultlyBackend.Api.Exceptions;
using VaultlyBackend.Api.Models.Dtos.Videos;
using VaultlyBackend.Api.Models.Entites;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Services
{
    public class VideoUploadService(VaultlyDbContext context, IWebHostEnvironment _env, IMapper mapper, IBackgroundTaskQueue taskQueue) : IVideoUploadService
    {


        public async Task<List<VideoDto>> GetVideos()
        {
            var videos = await context.Videos.ToListAsync();
            if (videos == null)
                throw new BusinessException("Video bulunamadı", "Video bulunamadı");
            var videoDtos = mapper.Map<List<VideoDto>>(videos);

            // Her video için URL’leri ekle
            foreach (var dto in videoDtos)
            {
                dto.ThumbnailUrl = $"/api/video/thumbnail/{dto.Id}/thumbnail.jpg";
                dto.HlsUrl = $"/api/video/stream/{dto.Id}/index.m3u8";
            }

            return videoDtos;
        }
        public async Task<VideoDto> VideoInit(string fileName)
        {
            var videoId = Guid.NewGuid();

            var video = new Video
            {
                Id = videoId,
                OriginalFileName = fileName
            };
           context.Videos.Add(video);

            await context.SaveChangesAsync();
            var chunkDir = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "chunks",
                videoId.ToString()
            );
            Directory.CreateDirectory(chunkDir);
            return mapper.Map<VideoDto>(video);
        }
        public async Task UploadChunk(Guid videoId, int index, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BusinessException("Yüklemede hata oluştu", "No Chunk");

            var chunkDir = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "chunks",
                videoId.ToString()
            );

            if (!Directory.Exists(chunkDir))
                throw new BusinessException("Upload init edilmemiş", "No Chunk");

            var chunkPath = Path.Combine(chunkDir, $"{index}.part");

            await using var fs = new FileStream(chunkPath, FileMode.Create);
            await file.CopyToAsync(fs);

        }
        public async Task<VideoDto> Complete(Guid videoId)
        {
            var chunkDir = Path.Combine(_env.ContentRootPath,"storage","chunks", videoId.ToString());

            if (!Directory.Exists(chunkDir))
                throw new BusinessException("Chunk klasörü yok", "Chunk klasörü yok");

            // ------------------------------------------------
            // 1️⃣ MERGE MP4 (kilit-safe, stream copy)
            // ------------------------------------------------
            var uploadDir = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "uploads"
            );
            Directory.CreateDirectory(uploadDir);

            var mergedPath = Path.Combine(uploadDir, $"{videoId}.mp4");

            // Eğer daha önce complete çağrıldıysa
            if (System.IO.File.Exists(mergedPath))
                throw new BusinessException("Video zaten birleştirilmiş", "Video zaten birleştirilmiş");

            var chunks = Directory.GetFiles(chunkDir)
                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)));

            await using (var output = new FileStream(
                mergedPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 1024 * 1024,
                useAsync: true))
            {
                foreach (var chunk in chunks)
                {
                    await using var input = new FileStream(
                        chunk,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 1024 * 1024,
                        useAsync: true
                    );

                    await input.CopyToAsync(output);
                }
            } // 🔥 burada dosya tamamen serbest

            // Küçük gecikme (Windows file lock edge case)
            await Task.Delay(200);

            // ------------------------------------------------
            // 2️⃣ HLS OUTPUT
            // ------------------------------------------------
            var hlsDir = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "hls",
                videoId.ToString()
            );
            Directory.CreateDirectory(hlsDir);

         
            taskQueue.QueueBackgroundWorkItem(async (serviceProvider, token) =>
            {
                await Task.Run(() => ConvertToHls(mergedPath, hlsDir), token);
                await Task.Run(() => GenerateThumbnail(mergedPath, hlsDir), token);
            });
         
            // ------------------------------------------------
            // 3️⃣ RESPONSE
            // ------------------------------------------------
            var video = await context.Videos.FindAsync(videoId);
            video.StreamUrl = $"/video/stream/{videoId}/index.m3u8";
           
            await context.SaveChangesAsync();
            return mapper.Map<VideoDto>(video);

        }
        public async Task<( string fullPath,string contentType)> Stream(Guid videoId, string file)
        {
            if (!Regex.IsMatch(file, @"^[a-zA-Z0-9._-]+$"))
                throw new BusinessException("Invalid file name", "Invalid file name");
            if (file.Contains("..") || file.Contains("/") || file.Contains("\\"))
                throw new BusinessException("Invalid file name", "Invalid file name");

            var baseDir = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "hls",
                videoId.ToString()
            );

            var fullPath = Path.GetFullPath(Path.Combine(baseDir, file));

            if (!fullPath.StartsWith(baseDir))
                throw new BusinessException("Invalid path", "Invalid path");

            if (!System.IO.File.Exists(fullPath))
                throw new BusinessException("NotFound", "NotFound");

            var contentType = file.EndsWith(".m3u8")
                ? "application/vnd.apple.mpegurl"
                : "video/mp2t";
            return(fullPath, contentType);
        }
        // HLS dönüştürme
        private async Task ConvertToHls(string inputPath, string outputDir)
        {
            var ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";
            var args = $"-y -i \"{inputPath}\" -c copy -start_number 0 -hls_time 6 -hls_list_size 0 -f hls \"{Path.Combine(outputDir, "index.m3u8")}\"";

            await RunFfmpeg(ffmpegPath, args, outputDir, "HLS");
        }

        // Thumbnail oluşturma
        private async Task GenerateThumbnail(string inputPath, string outputDir, string thumbnailName = "thumbnail.jpg")
        {
            var ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";
            var thumbnailPath = Path.Combine(outputDir, thumbnailName);
            var args = $"-y -i \"{inputPath}\" -ss 00:00:05 -vframes 1 -vf \"scale=320:180\" \"{thumbnailPath}\"";

            await RunFfmpeg(ffmpegPath, args, outputDir, "Thumbnail");
        }

        // Ortak FFMPEG çalıştırma
        private async Task RunFfmpeg(string ffmpegPath, string args, string workingDir, string taskName)
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
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"FFmpeg {taskName} failed: {error}");
            }
        }

       

    }
}
