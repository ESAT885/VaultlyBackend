namespace VaultlyBackend.Api.Background
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(
            IBackgroundTaskQueue taskQueue,
            ILogger<QueuedHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                BackgroundJob job = null!;

                try
                {
                    job = await _taskQueue.DequeueAsync(stoppingToken);

                    await job.WorkItem(stoppingToken);
                    await _taskQueue.MarkCompletedAsync(job);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Job failed");
                }
                finally
                {
                    if (job != null)
                    {
                        _taskQueue 
                            ?.MarkCompletedAsync(job);
                    }
                }
            }
        }
    }
}
