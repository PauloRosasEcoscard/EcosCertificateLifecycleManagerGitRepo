namespace EcosCLM.Domain.Entities.Base
{
    public class SessionEntry
    {
        public string Id { get; set; } = string.Empty;
        public byte[] Value { get; set; } = Array.Empty<byte>();
        public DateTime ExpiresAtTime { get; set; }
        public long? SlidingExpirationInSeconds { get; set; }
        public DateTime? AbsoluteExpiration { get; set; }
    }
}