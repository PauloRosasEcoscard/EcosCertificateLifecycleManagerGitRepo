using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Domain.Entities.Security;

namespace EcosCLM.Application.Interfaces
{
    public interface IHsmKeyRefRepository : IBaseRepository<HsmKeyRef, HsmKeyRefViewModel>
    {
    }
}