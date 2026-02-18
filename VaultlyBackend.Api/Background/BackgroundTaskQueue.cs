using System.Collections.Concurrent;
using System.Threading.Channels;

namespace VaultlyBackend.Api.Background
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<BackgroundJob> _queue;

        private readonly ConcurrentDictionary<Guid, BackgroundJob> _activeJobs
            = new();
        private readonly ConcurrentDictionary<Guid, BackgroundJob> _completedJobs = new();
        private readonly ConcurrentDictionary<Guid, BackgroundJob> _failedJobs = new();

        public BackgroundTaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            _queue = Channel.CreateBounded<BackgroundJob>(options);
        }

        public int QueueCount => _queue.Reader.Count;

        public IReadOnlyCollection<BackgroundJob> ActiveJobs
            => _activeJobs.Values.ToList();
        public IReadOnlyCollection<BackgroundJob> CompletedJobs => 
            _completedJobs.Values.ToList();
        public IReadOnlyCollection<BackgroundJob> FailedJobs => 
            _failedJobs.Values.ToList();

        public async ValueTask QueueAsync(BackgroundJob job)
        {
            job.Status = JobStatus.Queued;
            await _queue.Writer.WriteAsync(job);
        }

        public async ValueTask<BackgroundJob> DequeueAsync(CancellationToken cancellationToken)
        {
            var job = await _queue.Reader.ReadAsync(cancellationToken);

            job.Status = JobStatus.Running;
            job.StartedAt = DateTime.UtcNow;

            _activeJobs.TryAdd(job.Id, job);

            return job;
        }

        public async Task MarkCompletedAsync(BackgroundJob job)
        {
            job.Status = JobStatus.Completed;
            job.FinishedAt = DateTime.UtcNow;

            _activeJobs.TryRemove(job.Id, out _);
            _completedJobs.TryAdd(job.Id, job);

            await Task.CompletedTask;
        }

        public async Task MarkFailedAsync(BackgroundJob job)
        {
            job.RetryCount++;

            if (job.RetryCount <= job.MaxRetry)
            {
                job.Status = JobStatus.Queued;
                _activeJobs.TryRemove(job.Id, out _);

                await _queue.Writer.WriteAsync(job);
                return;
            }

            job.Status = JobStatus.Failed;
            job.FinishedAt = DateTime.UtcNow;

            _activeJobs.TryRemove(job.Id, out _);
            _failedJobs.TryAdd(job.Id, job);
        }
    }
}
