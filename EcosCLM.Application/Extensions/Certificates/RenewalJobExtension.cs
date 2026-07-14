using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Certificates
{
    public static class RenewalJobExtension
    {
        public static async Task<RenewalJobViewModel> GetByIdAsync(this IRenewalJobRepository repository, Guid id)
        {
            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(RenewalJob), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<RenewalJobViewModel>> GetAllWithPageAsync(this IRenewalJobRepository repository, int page = 0, int offset = 0, string filter = null, string oderBy = null, string orderDirection = null, Guid? customer = null)
        {
            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "due":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.DueAt) : query.OrderBy(i => i.DueAt);
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
                var search = JsonConvert.DeserializeObject<RenewalJobViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Status))
                        query = query.Where(x => x.Status == search.Status);

                    if (search.CertificateId != Guid.Empty)
                        query = query.Where(x => x.CertificateId == search.CertificateId);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync();
            return repository.ToListViewModel(list);
        }

        public static async Task<RenewalJobViewModel> CreateAsync(this IRenewalJobRepository repository, RenewalJob entity)
        {
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }
    }
}