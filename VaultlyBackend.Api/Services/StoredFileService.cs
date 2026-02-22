using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using VaultlyBackend.Api.Background;
using VaultlyBackend.Api.Data;
using VaultlyBackend.Api.Exceptions;
using VaultlyBackend.Api.Models.Dtos.StoredFiles;
using VaultlyBackend.Api.Models.Dtos.Videos;
using VaultlyBackend.Api.Models.Entites;
using VaultlyBackend.Api.Services.Interfaces;
using VaultlyBackend.Api.Strategies;
using VaultlyBackend.Api.Strategies.Interfaces;

namespace VaultlyBackend.Api.Services
{
    public class StoredFileService(VaultlyDbContext context, IWebHostEnvironment _env, IMapper mapper, IBackgroundTaskQueue taskQueue, IFfmpegVideoService ffmpegVideoService, IFileProcessorFactory filefactory) : IStoredFileService
    {
        public async Task<List<StoredFileDto>> GetStoredFiles()
        {
            var dataList = await context.StoredFiles.ToListAsync();
            return mapper.Map<List<StoredFileDto>>(dataList);
        }
        public async Task<StoredFileDto> FileInit(string fileName)
        {

            var storedFile = new StoredFile
            {

                Id = Guid.NewGuid(),
                OriginalFileName = fileName
            };
            context.StoredFiles.Add(storedFile);
            await context.SaveChangesAsync();
            if(string.IsNullOrEmpty(fileName))
                throw new BusinessException("Dosya adı boş olamaz", "Dosya adı boş olamaz");
            if (storedFile==null||storedFile?.Id==null)
                throw new BusinessException("Dosya kayıt olamadı", "Dosya kayıt olamadı");


            string storedFilefileName = SetFileName(storedFile.Id,fileName);
            var chunkDir = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "chunks",
                storedFilefileName
            );
            Directory.CreateDirectory(chunkDir);
            return mapper.Map<StoredFileDto>(storedFile);
        }
        public async Task UploadChunk(Guid storedFileId, int index, IFormFile file)
        {
            var storedFile = await context.StoredFiles.FindAsync(storedFileId);
            if (file == null || file.Length == 0)
                throw new BusinessException("Yüklemede hata oluştu", "No Chunk");
            string storedFilefileName = SetFileName(storedFileId, storedFile?.OriginalFileName??"");
            var chunkDir = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "chunks",
                storedFilefileName
            );

            if (!Directory.Exists(chunkDir))
                throw new BusinessException("Upload init edilmemiş", "No Chunk");

            var chunkPath = Path.Combine(chunkDir, $"{index}.part");

            await using var fs = new FileStream(chunkPath, FileMode.Create);
            await file.CopyToAsync(fs);

        }
        public async Task<StoredFileDto> Complete(Guid storedFileId)
        {
            var storedFile = await context.StoredFiles.FindAsync(storedFileId);
            string storedFilefileName = SetFileName(storedFileId, storedFile?.OriginalFileName ?? "");

            var chunkDir = Path.Combine(_env.ContentRootPath, "storage", "chunks", storedFilefileName);

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
           
            if (storedFile == null|| storedFile?.OriginalFileName==null)
                throw new BusinessException("Dosya bulunamadı", "Dosya bulunamadı");
            
            var mergedPath = Path.Combine(uploadDir, storedFilefileName);

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


            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(mergedPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var processor = filefactory.GetProcessor(contentType);

           await processor.ProcessAsync(mergedPath,storedFilefileName);

            // ------------------------------------------------
            // 2️⃣ HLS OUTPUT
            // ------------------------------------------------
            //var hlsDir = Path.Combine(
            //    _env.ContentRootPath,
            //    "storage",
            //    "hls",
            //    storedFilefileName
            //);
            //Directory.CreateDirectory(hlsDir);

            //var processor = filefactory.GetProcessor(contentType);

            //await processor.ProcessAsync(mergedPath, storedFilefileName);

            //await taskQueue.QueueAsync(new BackgroundJob
            //{
            //    Name = $"HLS Dönüştürme - VideoId: "+ storedFilefileName,
            //    WorkItem = async token =>
            //    {
            //        await ffmpegVideoService.ConvertToHls(mergedPath, hlsDir);
            //    }
            //});
            //await taskQueue.QueueAsync(new BackgroundJob
            //{
            //    Name = $"Thumbnail Oluşturma - VideoId: "+ storedFilefileName,
            //    WorkItem = async token =>
            //    {
            //        await ffmpegVideoService.GenerateThumbnail(mergedPath, hlsDir);
            //    }
            //});
            DeleteChunks(chunkDir);
            return mapper.Map<StoredFileDto>(storedFile);

        }

        public async Task<(string fileName,string filePath)>DownLoad(Guid storedFileId)
        {

            var storedFile = await context.StoredFiles.FindAsync(storedFileId);
            if (storedFile == null || string.IsNullOrEmpty(storedFile.OriginalFileName))
                throw new BusinessException("Dosya bulunamadı", "Dosya bulunamadı");
            var storedFilefileName = SetFileName(storedFileId, storedFile.OriginalFileName);
            var filePath = Path.Combine(
                _env.ContentRootPath,
                "storage",
                "uploads",
                storedFilefileName
            );
            if (!System.IO.File.Exists(filePath))
                throw new BusinessException("Dosya henüz hazır değil", "Dosya henüz hazır değil");
            return (storedFile.OriginalFileName, filePath);
        }
        private string SetFileName(Guid storedFileId, string fileName)
        {
            return storedFileId.ToString() + "_" + fileName;
        }
        private void DeleteChunks(string chunkDir)
        {
            if (!Directory.Exists(chunkDir))
                return;

            try
            {
                var files = Directory.GetFiles(chunkDir);

                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }

                Directory.Delete(chunkDir);
            }
            catch (Exception ex)
            {
                // Logla ama sistemi durdurma
                Console.WriteLine($"Chunk silme hatası: {ex.Message}");
            }
        }
    }
}