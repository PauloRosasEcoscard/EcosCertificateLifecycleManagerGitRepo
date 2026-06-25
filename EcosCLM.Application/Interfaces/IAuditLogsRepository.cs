using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.Interfaces
{
    public interface IAuditLogsRepository : IBaseRepository<AuditLogs, AuditLogsViewModel>
    {
    }
}
