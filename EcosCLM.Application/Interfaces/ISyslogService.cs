using EcosCLM.Application.Services;

namespace EcosCLM.Application.Interfaces
{
    public interface ISyslogService
    {
        string SendLog(string appName, string message, SyslogSeverity severity = SyslogSeverity.Notice);
        string SendLog(string appName, object auditLogs, SyslogSeverity severity = SyslogSeverity.Notice);
        Task InitializeAsync(Guid idCustomer);
        string TestConnection();
    }
}
