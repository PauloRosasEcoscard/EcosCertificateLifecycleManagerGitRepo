using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Integration;

namespace EcosCLM.Data.Repositories
{
    public class EventOutboxRepository : BaseRepository<EventOutbox, EventOutboxViewModel>, IEventOutboxRepository
    {
        public EventOutboxRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}