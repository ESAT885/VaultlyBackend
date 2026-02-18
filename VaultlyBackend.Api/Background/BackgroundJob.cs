namespace VaultlyBackend.Api.Background
{
    public class BackgroundJob
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public string Name { get; set; } = string.Empty;
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public TimeSpan? ExecutionDuration =>
            StartedAt.HasValue && FinishedAt.HasValue
                ? FinishedAt - StartedAt
                : null;
        public JobStatus Status { get; set; } = JobStatus.Queued;

        public int RetryCount { get; set; } = 0;

        public int MaxRetry { get; set; } = 3;

        public Func<CancellationToken, ValueTask> WorkItem { get; set; } = default!;
    }
    public enum JobStatus
    {
        Queued = 0,
        Running = 1,
        Completed = 2,
        Failed = 3
    }
}
