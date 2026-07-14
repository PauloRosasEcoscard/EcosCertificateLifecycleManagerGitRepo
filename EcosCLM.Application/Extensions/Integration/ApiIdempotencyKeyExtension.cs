using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Integration
{
    public static class ApiIdempotencyKeyExtension
    {
        public static async Task<ApiIdempotencyKeyViewModel> GetByIdAsync(this IApiIdempotencyKeyRepository repository, Guid id)
        {
            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(ApiIdempotencyKey), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<ApiIdempotencyKeyViewModel>> GetAllWithPageAsync(this IApiIdempotencyKeyRepository repository, int page = 0, int offset = 0, string filter = null, string oderBy = null, string orderDirection = null, Guid? customer = null)
        {
            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "key":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Key) : query.OrderBy(i => i.Key);
                        break;
                    default:
                        query = query.OrderByDescending(x => x.ExpiresAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.ExpiresAt);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var search = JsonConvert.DeserializeObject<ApiIdempotencyKeyViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Key))
                        query = query.Where(x => x.Key == search.Key);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync();
            return repository.ToListViewModel(list);
        }

        public static async Task<ApiIdempotencyKeyViewModel> CreateAsync(this IApiIdempotencyKeyRepository repository, ApiIdempotencyKey entity)
        {
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }
    }
}