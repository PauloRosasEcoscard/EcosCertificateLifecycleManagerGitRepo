using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Security;

namespace EcosCLM.Data.Repositories
{
    public class HsmKeyRefRepository : BaseRepository<HsmKeyRef, HsmKeyRefViewModel>, IHsmKeyRefRepository
    {
        public HsmKeyRefRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}