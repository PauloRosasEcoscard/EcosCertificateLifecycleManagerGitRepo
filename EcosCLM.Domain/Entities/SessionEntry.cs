namespace EcosCLM.Domain.Entities
{
    public class SessionEntry
    {
        public string Id { get; set; }
        public byte[] Value { get; set; }
        public DateTime ExpiresAtTime { get; set; }
        public long? SlidingExpirationInSeconds { get; set; }
        public DateTime? AbsoluteExpiration { get; set; }
    }
}
