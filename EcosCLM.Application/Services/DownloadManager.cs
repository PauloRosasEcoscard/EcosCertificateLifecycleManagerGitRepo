using EcosCLM.Application.Interfaces;
using EcosCLM.Domain.Entities.Base;
using Microsoft.Extensions.DependencyInjection;

namespace EcosCLM.Application.Services
{
    public class DownloadManager : IDownloadManager
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBackgroundTaskQueue _queue;

        public DownloadManager(
            IServiceScopeFactory scopeFactory,
            IBackgroundTaskQueue queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        public async Task<Guid> EnqueueAsync(
            string user,
            DownloadGenerator generator,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(generator);

            using var scope = _scopeFactory.CreateScope();

            var _repository =
                scope.ServiceProvider.GetRequiredService<IDownloadJobsRepository>();

            var job = new DownloadJobs
            {
                Id = Guid.NewGuid(),
                User = user,
                ContentType = string.Empty,
                FilePath = string.Empty,
                FileName = string.Empty,
                Error = string.Empty,
                Status = DownloadStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(job).ConfigureAwait(false);

            await _queue.QueueBackgroundWorkItemAsync(async token =>
            {
                using var innerScope = _scopeFactory.CreateScope();
                var manager = innerScope.ServiceProvider.GetRequiredService<IDownloadManager>();

                await manager.ProcessAsync(job.Id, generator, token).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return job.Id;
        }

        public async Task ProcessAsync(
            Guid jobId,
            DownloadGenerator generator,
            CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(generator);

            using var scope = _scopeFactory.CreateScope();

            var _repository =
                scope.ServiceProvider.GetRequiredService<IDownloadJobsRepository>();

            var _notifications =
                scope.ServiceProvider.GetRequiredService<INotificationsRepository>();

            var job = await _repository.FindOneAsync(x => x.Id == jobId).ConfigureAwait(false);

            if (job == null)
                return;

            job.Status = DownloadStatus.Processing;
            await _repository.UpdAsync(job).ConfigureAwait(false);

            try
            {
                var file = await generator(ct).ConfigureAwait(false);

                var path = Path.Combine("downloads", jobId.ToString());
                Directory.CreateDirectory(path);

                var filePath = Path.Combine(path, file.FileName);
                await File.WriteAllBytesAsync(filePath, file.Content, ct).ConfigureAwait(false);

                job.FilePath = filePath;
                job.FileName = file.FileName;
                job.ContentType = file.ContentType;
                job.Status = DownloadStatus.Ready;
                job.FinishedAt = DateTime.UtcNow;

                await _notifications.AddAsync(new Notifications
                {
                    Timestamp = DateTime.Now,
                    User = job.User,
                    Message = $"Your file \"{job.FileName}\" is ready for download.",
                    Link = $"/Download?id={job.Id}",
                    Icon = "download"
                }).ConfigureAwait(false);

                await _repository.UpdAsync(job).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                job.Status = DownloadStatus.Error;
                job.Error = ex.Message;
                await _repository.UpdAsync(job).ConfigureAwait(false);
            }
        }
    }
}