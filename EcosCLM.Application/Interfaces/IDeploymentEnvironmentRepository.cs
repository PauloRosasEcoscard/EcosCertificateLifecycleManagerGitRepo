using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Domain.Entities.Catalog;

namespace EcosCLM.Application.Interfaces
{
    public interface IDeploymentEnvironmentRepository : IBaseRepository<DeploymentEnvironment, DeploymentEnvironmentViewModel>
    {
    }
}