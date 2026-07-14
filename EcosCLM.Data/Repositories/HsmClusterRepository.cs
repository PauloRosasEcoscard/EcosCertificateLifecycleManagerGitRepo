using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Security;

namespace EcosCLM.Data.Repositories
{
    public class HsmClusterRepository : BaseRepository<HsmCluster, HsmClusterViewModel>, IHsmClusterRepository
    {
        public HsmClusterRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}