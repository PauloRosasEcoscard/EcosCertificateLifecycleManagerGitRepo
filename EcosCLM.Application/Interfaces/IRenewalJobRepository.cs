using EcosCLM.Application.ViewModels;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Domain.Entities.Certificates;

namespace EcosCLM.Application.Interfaces
{

    public interface IRenewalJobRepository : IBaseRepository<RenewalJob, RenewalJobViewModel>
    {
    }
}