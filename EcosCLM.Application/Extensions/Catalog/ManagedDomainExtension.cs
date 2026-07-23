using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Catalog
{
    public static class ManagedDomainExtension
    {
        public static async Task<ManagedDomainViewModel> GetByIdAsync(this IManagedDomainRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(ManagedDomain), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<ManagedDomainViewModel>> GetAllWithPageAsync(this IManagedDomainRepository repository, int page = 0, int offset = 0, string? filter = null, string? oderBy = null, string? orderDirection = null, Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "fqdn":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Fqdn) : query.OrderBy(i => i.Fqdn);
                        break;

                    default:
                        query = query.OrderByDescending(x => x.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedAt);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var search = JsonConvert.DeserializeObject<ManagedDomainViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Fqdn))
                        query = query.Where(x => x.Fqdn.Contains(search.Fqdn));

                    if (!string.IsNullOrEmpty(search.ValidationStatus))
                        query = query.Where(x => x.ValidationStatus == search.ValidationStatus);

                    if (search.ApplicationId.HasValue && search.ApplicationId.Value != Guid.Empty)
                        query = query.Where(x => x.ApplicationId == search.ApplicationId.Value);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<ManagedDomainViewModel> CreateAsync(this IManagedDomainRepository repository, ManagedDomain entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }
    }
}