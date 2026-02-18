using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VaultlyBackend.Api.Background;

namespace VaultlyBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundMonitorController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _queue;

        public BackgroundMonitorController(IBackgroundTaskQueue queue)
        {
            _queue = queue;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                QueueCount = _queue.QueueCount,
                Active = _queue.ActiveJobs.Select(Map),
                Completed = _queue.CompletedJobs.Select(Map),
                Failed = _queue.FailedJobs.Select(Map)
            });
        }

        private object Map(BackgroundJob job) => new
        {
            job.Id,
            job.Name,
            job.Status,
            job.RetryCount,
            job.CreatedAt,
            job.StartedAt,
            job.FinishedAt,
            DurationSeconds = job.ExecutionDuration?.TotalSeconds
        };
    }
}
