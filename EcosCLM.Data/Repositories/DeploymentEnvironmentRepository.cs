using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Catalog;

namespace EcosCLM.Data.Repositories
{
    public class DeploymentEnvironmentRepository : BaseRepository<DeploymentEnvironment, DeploymentEnvironmentViewModel>, IDeploymentEnvironmentRepository
    {
        public DeploymentEnvironmentRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}