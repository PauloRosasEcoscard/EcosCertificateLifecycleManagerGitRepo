using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Catalog;

namespace EcosCLM.Data.Repositories
{
    public class ManagedDomainRepository : BaseRepository<ManagedDomain, ManagedDomainViewModel>, IManagedDomainRepository
    {
        public ManagedDomainRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}