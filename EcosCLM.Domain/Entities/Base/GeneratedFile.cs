namespace EcosCLM.Domain.Entities.Base
{
    public class GeneratedFile
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }

    public delegate Task<GeneratedFile> DownloadGenerator(CancellationToken ct);
}