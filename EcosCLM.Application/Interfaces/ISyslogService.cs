using EcosCLM.Application.Services;

namespace EcosCLM.Application.Interfaces
{
    public interface ISyslogService
    {
        public string SendLog(string appName, string message, SyslogSeverity severity = SyslogSeverity.Notice);
        public string SendLog(string appName, object auditLogs, SyslogSeverity severity = SyslogSeverity.Notice);
        public Task InitializeAsync(Guid idCustomer);
        public string TestConnection();
    }
}
