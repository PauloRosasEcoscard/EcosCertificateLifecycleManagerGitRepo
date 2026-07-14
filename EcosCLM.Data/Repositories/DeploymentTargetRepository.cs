using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Deployment;

namespace EcosCLM.Data.Repositories
{
    public class DeploymentTargetRepository : BaseRepository<DeploymentTarget, DeploymentTargetViewModel>, IDeploymentTargetRepository
    {
        public DeploymentTargetRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}