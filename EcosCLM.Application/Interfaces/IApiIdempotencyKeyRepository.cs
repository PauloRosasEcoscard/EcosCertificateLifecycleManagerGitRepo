using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;

namespace EcosCLM.Application.Interfaces
{
    public interface IApiIdempotencyKeyRepository : IBaseRepository<ApiIdempotencyKey, ApiIdempotencyKeyViewModel>
    {
    }
}