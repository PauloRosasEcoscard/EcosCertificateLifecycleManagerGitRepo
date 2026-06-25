namespace EcosCLM.Domain.Entities
{
    public class GeneratedFile
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }

    public delegate Task<GeneratedFile> DownloadGenerator(CancellationToken ct);
}
