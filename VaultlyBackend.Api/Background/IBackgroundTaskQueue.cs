namespace VaultlyBackend.Api.Background
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueAsync(BackgroundJob job);
        ValueTask<BackgroundJob> DequeueAsync(CancellationToken cancellationToken);

        int QueueCount { get; }
        public Task MarkCompletedAsync(BackgroundJob job);
        public Task MarkFailedAsync(BackgroundJob job);
        IReadOnlyCollection<BackgroundJob> ActiveJobs { get; }
        IReadOnlyCollection<BackgroundJob> CompletedJobs { get; }
        IReadOnlyCollection<BackgroundJob> FailedJobs { get; }
    }
}

