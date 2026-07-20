using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.Interfaces
{
    public interface IDownloadManager
    {
        public Task<Guid> EnqueueAsync(
            string user,
            DownloadGenerator generator,
            CancellationToken ct = default);

        public Task ProcessAsync(
            Guid jobId,
            DownloadGenerator generator,
            CancellationToken ct);
    }

}
