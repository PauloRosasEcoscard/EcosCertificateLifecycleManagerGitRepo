using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Integration;

namespace EcosCLM.Data.Repositories
{
    public class ApiIdempotencyKeyRepository : BaseRepository<ApiIdempotencyKey, ApiIdempotencyKeyViewModel>, IApiIdempotencyKeyRepository
    {
        public ApiIdempotencyKeyRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}