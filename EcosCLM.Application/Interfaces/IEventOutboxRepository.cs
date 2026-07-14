using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;

namespace EcosCLM.Application.Interfaces
{
    public interface IEventOutboxRepository : IBaseRepository<EventOutbox, EventOutboxViewModel>
    {
    }
}