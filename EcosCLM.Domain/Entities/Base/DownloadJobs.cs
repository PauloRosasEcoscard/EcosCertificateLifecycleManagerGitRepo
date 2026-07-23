namespace EcosCLM.Domain.Entities.Base
{
    public class DownloadJobs
    {
        public Guid Id { get; set; }
        public string User { get; set; } = string.Empty;
        public DownloadStatus Status { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? Error { get; set; }
    }

    public enum DownloadStatus
    {
        Pending = 0,
        Processing = 1,
        Ready = 2,
        Error = 3
    }
}