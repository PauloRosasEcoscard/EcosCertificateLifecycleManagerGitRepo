using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Data.Repositories
{
    public class SessionEntryRepository : BaseRepository<SessionEntry, SessionEntryViewModel>, ISessionEntryRepository
    {
        public SessionEntryRepository(EcosCLMContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }
    }
}
