using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VaultlyBackend.Api.Models.Dtos.StoredFiles;
using VaultlyBackend.Api.Models.Dtos.Videos;
using VaultlyBackend.Api.Models.Entites;
using VaultlyBackend.Api.Services;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoredFilesController(IStoredFileService storedFileService, IWebHostEnvironment _env, IMemoryCache _cache) : BaseController
    {
        [HttpGet()]
        public async Task<IActionResult> Videos()
        {
            var result = await storedFileService.GetStoredFiles();
           
            return Success<List<StoredFileDto>>(result);
        }
        [HttpGet("download-link/{storedFileId}")]
        public IActionResult GetDownloadLink(Guid storedFileId)
        {
            var token = Guid.NewGuid().ToString();

            // 5 dakika geçerli
            _cache.Set(token, storedFileId, TimeSpan.FromMinutes(5));

            var url = $"{Request.Scheme}://{Request.Host}/api/StoredFiles/download-temp/{token}";

            
            return Success(new { url });
        }

        // 🔓 Temporary public endpoint
        [HttpGet("download-temp/{token}")]
        public async Task<IActionResult> DownloadTemp(string token)
        {
            if (!_cache.TryGetValue(token, out Guid storedFileId))
                return Unauthorized("Link expired");

            var file = await storedFileService.DownLoad(storedFileId);

            if (file.fileName == null)
                return NotFound();

            return PhysicalFile(
                file.filePath,
                "application/octet-stream",
                file.fileName,
                enableRangeProcessing: true // 🎥 video için kritik
            );
        }
        [HttpPost("init")]
        public async Task<IActionResult> Init([FromQuery] string fileName)
        {
            var result = await storedFileService.FileInit(fileName);
            return Success<StoredFileDto?>(result);
        }


        [DisableRequestSizeLimit]
        [HttpPost("chunk")]
        public async Task<IActionResult> UploadChunk(
            [FromForm] Guid storedFileId,
            [FromForm] int index,
            [FromForm] IFormFile file)
        {
            await storedFileService.UploadChunk(storedFileId, index, file);
            return Success("chunk yüklendi");

        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete(Guid storedFileId)
        {
            var result = await storedFileService.Complete(storedFileId);
            return Success<StoredFileDto?>(result);

        }
    }
}
