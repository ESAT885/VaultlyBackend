using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VaultlyBackend.Api.Models.Dtos.Videos;
using VaultlyBackend.Api.Models.Entites;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController(IVideoUploadService videoUploadService, IWebHostEnvironment _env) : BaseController
    {

        [HttpGet()]
        public async Task<IActionResult> Videos()
        {
            var result = await videoUploadService.GetVideos();
            foreach (var video in result)
            {
                var thumbFile = "thumbnail.jpg";
                var thumbPath = Path.Combine(_env.ContentRootPath, "storage", "hls", video.Id.ToString(), thumbFile);

                if (System.IO.File.Exists(thumbPath))
                {
                    video.ThumbnailUrl = GetThumbnailUrl(video.Id);

                }
            }
            return Success<List<VideoDto>>(result);
        }

        private string GetThumbnailUrl(Guid videoId)
        {
            string host;

            if (_env.IsProduction())
            {
                // Production’da gerçek host’u kullan
                host = Request.Host.Host;
            }
            else
            {
                // Development / Staging → localhost kullan
                host = "localhost";
            }

            var port = Request?.HttpContext?.Connection?.LocalPort ?? 5000;
            var scheme = Request?.Scheme ?? "http";
            

            return $"{scheme}://{host}:{port}/api/Video/thumbnail/{videoId}/thumbnail.jpg";
        }

        [HttpPost("init")]
        public async Task<IActionResult> Init([FromQuery] string fileName)
        {
            var result =await videoUploadService.VideoInit(fileName);
           return Success<VideoDto?>(result);
        }
        [HttpGet("thumbnail/{videoId}/{file}")]
        public IActionResult GetThumbnail(Guid videoId, string file)
        {
            var path = Path.Combine(_env.ContentRootPath, "storage", "hls", videoId.ToString(), file);

            if (!System.IO.File.Exists(path))
                return NotFound();

            return PhysicalFile(path, "image/jpeg");
        }


        [DisableRequestSizeLimit]
        [HttpPost("chunk")]
        public async Task<IActionResult> UploadChunk(
            [FromForm] Guid videoId,
            [FromForm] int index,
            [FromForm] IFormFile file)
        {
             await videoUploadService.UploadChunk(videoId,index,file);
            return Success("chunk yüklendi");
           
        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete(Guid videoId)
        {
          var result=  await videoUploadService.Complete(videoId);
            return Success<VideoDto?>(result);

        }

       
        [HttpGet("stream/{videoId}/{file}")]
        public async Task<IActionResult> Stream(Guid videoId, string file)
        {
            var result=await videoUploadService.Stream(videoId,file);
            return PhysicalFile(result.fullPath, result.contentType, enableRangeProcessing: true);
           
        }


       
    }
}
