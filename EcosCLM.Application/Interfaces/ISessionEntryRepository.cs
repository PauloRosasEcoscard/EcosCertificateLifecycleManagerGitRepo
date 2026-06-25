using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.Interfaces
{
    public interface ISessionEntryRepository : IBaseRepository<SessionEntry, SessionEntryViewModel>
    {
    }
}
