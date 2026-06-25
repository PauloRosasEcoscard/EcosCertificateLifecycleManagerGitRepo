using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.Interfaces
{
    public interface IDownloadManager
    {
        Task<Guid> EnqueueAsync(
            string user,
            DownloadGenerator generator,
            CancellationToken ct = default);

        Task ProcessAsync(
            Guid jobId,
            DownloadGenerator generator,
            CancellationToken ct);
    }

}
