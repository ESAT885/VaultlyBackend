using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VaultlyBackend.Api.Models.Dtos.Videos;
using VaultlyBackend.Api.Services.Interfaces;
#region ESKİ KOD
//using System.Diagnostics;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using VaultlyBackend.Api.Models.Auth;
//using VaultlyBackend.Api.Models.Dtos;
//using VaultlyBackend.Api.Services.Interfaces;

//namespace VaultlyBackend.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class VideoController(IVideoUploadService videoUploadService) : BaseController
//    {
//        //private readonly IWebHostEnvironment _env;

//        //public VideoController(IWebHostEnvironment env)
//        //{
//        //    _env = env;
//        //}

//        // =========================================================
//        // 1️⃣ UPLOAD INIT
//        // POST /video/init?fileName=test.mp4
//        // =========================================================
//        [HttpPost("init")]
//        public async Task<IActionResult> Init([FromQuery] string fileName)
//        {
//            //var videoId = Guid.NewGuid();

//            //var chunkDir = Path.Combine(
//            //    _env.ContentRootPath,
//            //    "storage",
//            //    "chunks",
//            //    videoId.ToString()
//            //);

//            //Directory.CreateDirectory(chunkDir);

//            //return Ok(new { videoId });
//            var result = await videoUploadService.VideoInit(fileName);
//            return Success<VideoDto?>(result);
//        }

//        // =========================================================
//        // 2️⃣ CHUNK UPLOAD
//        // POST /video/chunk?videoId=...&index=0
//        // =========================================================
//        [DisableRequestSizeLimit]
//        [HttpPost("chunk")]
//        public async Task<IActionResult> UploadChunk(
//            [FromForm] Guid videoId,
//            [FromForm] int index,
//            [FromForm] IFormFile file)
//        {
//            await videoUploadService.UploadChunk(videoId, index, file);
//            return Success("chunk yüklendi");
//            //if (file == null || file.Length == 0)
//            //    return BadRequest("Chunk yok");

//            //var chunkDir = Path.Combine(
//            //    _env.ContentRootPath,
//            //    "storage",
//            //    "chunks",
//            //    videoId.ToString()
//            //);

//            //if (!Directory.Exists(chunkDir))
//            //    return NotFound("Upload init edilmemiş");

//            //var chunkPath = Path.Combine(chunkDir, $"{index}.part");

//            //await using var fs = new FileStream(chunkPath, FileMode.Create);
//            //await file.CopyToAsync(fs);

//            //return Ok();
//        }

//        // =========================================================
//        // 3️⃣ COMPLETE → MERGE + FFMPEG → HLS
//        // POST /video/complete?videoId=...
//        // =========================================================
//        [HttpPost("complete")]
//        public async Task<IActionResult> Complete(Guid videoId)
//        {
//            var result = await videoUploadService.Complete(videoId);
//            return Success<VideoDto?>(result);
//            //            var chunkDir = Path.Combine(
//            //    _env.ContentRootPath,
//            //    "storage",
//            //    "chunks",
//            //    videoId.ToString()
//            //);

//            //            if (!Directory.Exists(chunkDir))
//            //                return NotFound("Chunk klasörü yok");

//            //            // ------------------------------------------------
//            //            // 1️⃣ MERGE MP4 (kilit-safe, stream copy)
//            //            // ------------------------------------------------
//            //            var uploadDir = Path.Combine(
//            //                _env.ContentRootPath,
//            //                "storage",
//            //                "uploads"
//            //            );
//            //            Directory.CreateDirectory(uploadDir);

//            //            var mergedPath = Path.Combine(uploadDir, $"{videoId}.mp4");

//            //            // Eğer daha önce complete çağrıldıysa
//            //            if (System.IO.File.Exists(mergedPath))
//            //                return BadRequest("Video zaten birleştirilmiş");

//            //            var chunks = Directory.GetFiles(chunkDir)
//            //                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)));

//            //            await using (var output = new FileStream(
//            //                mergedPath,
//            //                FileMode.Create,
//            //                FileAccess.Write,
//            //                FileShare.None,
//            //                bufferSize: 1024 * 1024,
//            //                useAsync: true))
//            //            {
//            //                foreach (var chunk in chunks)
//            //                {
//            //                    await using var input = new FileStream(
//            //                        chunk,
//            //                        FileMode.Open,
//            //                        FileAccess.Read,
//            //                        FileShare.Read,
//            //                        bufferSize: 1024 * 1024,
//            //                        useAsync: true
//            //                    );

//            //                    await input.CopyToAsync(output);
//            //                }
//            //            } // 🔥 burada dosya tamamen serbest

//            //            // Küçük gecikme (Windows file lock edge case)
//            //            await Task.Delay(200);

//            //            // ------------------------------------------------
//            //            // 2️⃣ HLS OUTPUT
//            //            // ------------------------------------------------
//            //            var hlsDir = Path.Combine(
//            //                _env.ContentRootPath,
//            //                "storage",
//            //                "hls",
//            //                videoId.ToString()
//            //            );
//            //            Directory.CreateDirectory(hlsDir);

//            //            await ConvertToHls(mergedPath, hlsDir);

//            //            // ------------------------------------------------
//            //            // 3️⃣ RESPONSE
//            //            // ------------------------------------------------
//            //            return Ok(new
//            //            {
//            //                videoId,
//            //                streamUrl = $"/video/stream/{videoId}/index.m3u8"
//            //            });

//        }

//        // =========================================================
//        // 4️⃣ STREAM HLS
//        // GET /video/stream/{videoId}/index.m3u8
//        // =========================================================
//        [HttpGet("stream/{videoId}/{file}")]
//        public async Task<IActionResult> Stream(Guid videoId, string file)
//        {
//            var result = await videoUploadService.Stream(videoId, file);

//            return PhysicalFile(result.fullPath, result.contentType, enableRangeProcessing: true);
//            //if (!Regex.IsMatch(file, @"^[a-zA-Z0-9._-]+$"))
//            //    return BadRequest("Invalid file name");
//            //if (file.Contains("..") || file.Contains("/") || file.Contains("\\"))
//            //    return BadRequest("Invalid file name");

//            //var baseDir = Path.Combine(
//            //    _env.ContentRootPath,
//            //    "storage",
//            //    "hls",
//            //    videoId.ToString()
//            //);

//            //var fullPath = Path.GetFullPath(Path.Combine(baseDir, file));

//            //if (!fullPath.StartsWith(baseDir))
//            //    return BadRequest("Invalid path");

//            //if (!System.IO.File.Exists(fullPath))
//            //    return NotFound();

//            //var contentType = file.EndsWith(".m3u8")
//            //    ? "application/vnd.apple.mpegurl"
//            //    : "video/mp2t";

//            //return PhysicalFile(fullPath, contentType, enableRangeProcessing: true);
//        }


//        // =========================================================
//        // 🔧 FFMPEG
//        // =========================================================
//        //private async Task ConvertToHls(string inputPath, string outputDir)
//        //{
//        //    var ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";

//        //    var args =
//        //$"-y -i \"{inputPath}\" " +
//        //"-c copy " +                 // re-encode yok → hızlı
//        //"-start_number 0 " +
//        //"-hls_time 6 " +
//        //"-hls_list_size 0 " +
//        //"-f hls " +
//        //$"\"{Path.Combine(outputDir, "index.m3u8")}\"";

//        //    var process = new Process
//        //    {
//        //        StartInfo = new ProcessStartInfo
//        //        {
//        //            FileName = ffmpegPath,
//        //            Arguments = args,
//        //            WorkingDirectory = outputDir,
//        //            RedirectStandardOutput = true,
//        //            RedirectStandardError = true,
//        //            UseShellExecute = false,
//        //            CreateNoWindow = true
//        //        }
//        //    };

//        //    process.Start();
//        //    await process.WaitForExitAsync();
//        //    if (process.ExitCode != 0)
//        //    {
//        //        var error = await process.StandardError.ReadToEndAsync();
//        //        throw new Exception("FFmpeg HLS failed: " + error);
//        //    }

//        //}
//    }
//}

#endregion
namespace VaultlyBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController(IVideoUploadService videoUploadService) : BaseController
    {

        [HttpGet()]
        public async Task<IActionResult> Videos()
        {
            var result = await videoUploadService.GetVideos();
            return Success<List<VideoDto>>(result);
        }
        [HttpPost("init")]
        public async Task<IActionResult> Init([FromQuery] string fileName)
        {
            var result =await videoUploadService.VideoInit(fileName);
           return Success<VideoDto?>(result);
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
