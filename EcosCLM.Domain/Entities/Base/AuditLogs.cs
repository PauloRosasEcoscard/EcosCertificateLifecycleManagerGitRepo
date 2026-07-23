namespace EcosCLM.Domain.Entities.Base
{
    public class AuditLogs
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; } = string.Empty;
        public Guid IdCustumer { get; set; }
        public string LogType { get; set; } = string.Empty;
        public string Log { get; set; } = string.Empty;
        public string SourceIp { get; set; } = string.Empty;
        public string DestinationIp { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
    }
}