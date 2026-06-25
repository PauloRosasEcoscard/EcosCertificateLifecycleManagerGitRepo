using EcosCLM.Application.Services;

namespace EcosCLM.Application.Interfaces
{
    public interface ISyslogService
    {
        string SendLog(string appName, string message, SyslogSeverity severity = SyslogSeverity.Notice);
        string SendLog(string appName, Object auditLogs, SyslogSeverity severity = SyslogSeverity.Notice);
        void Initialize(Guid IdCustumer);
        string TestConnection();
    }
}
