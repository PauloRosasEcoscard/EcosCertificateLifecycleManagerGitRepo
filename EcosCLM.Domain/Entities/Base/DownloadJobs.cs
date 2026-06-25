namespace EcosCLM.Domain.Entities.Base
{
    public class DownloadJobs
    {
        public Guid Id { get; set; }
        public string User {  get; set; }
        public DownloadStatus Status { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string Error { get; set; }
    }

    public enum DownloadStatus
    {
        Pending = 0,
        Processing = 1,
        Ready = 2,
        Error = 3
    }
}
