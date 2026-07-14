using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Domain.Entities.Catalog;

namespace EcosCLM.Application.Interfaces
{
    public interface ICLMApplicationRepository : IBaseRepository<CLMApplication, CLMApplicationViewModel>
    {
    }
}