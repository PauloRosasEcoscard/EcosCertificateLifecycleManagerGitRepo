using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Data.Repositories
{
    public class AuditLogsRepository : BaseRepository<AuditLogs, AuditLogsViewModel>, IAuditLogsRepository
    {
        public AuditLogsRepository(EcosDashboardContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }
    }
}
