using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Domain.Entities.Deployment;

namespace EcosCLM.Application.Interfaces
{
    public interface IDeploymentTargetRepository : IBaseRepository<DeploymentTarget, DeploymentTargetViewModel>
    {
    }
}