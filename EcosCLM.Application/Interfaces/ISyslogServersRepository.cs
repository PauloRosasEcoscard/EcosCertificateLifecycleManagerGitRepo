using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;

namespace EcosCLM.Application.Interfaces
{
    public interface ISyslogServersRepository : IBaseRepository<SyslogServers, SyslogServersViewModel>
    {
    }
}
