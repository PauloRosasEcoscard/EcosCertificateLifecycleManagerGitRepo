using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Certificates
{
    public static class CertificateRequestExtension
    {
        public static async Task<CertificateRequestViewModel> GetByIdAsync(this ICertificateRequestRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(CertificateRequest), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<CertificateRequestViewModel>> GetAllWithPageAsync(this ICertificateRequestRepository repository, int page = 0, int offset = 0, string? filter = null, string? oderBy = null, string? orderDirection = null, Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "status":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Status) : query.OrderBy(i => i.Status);
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
                var search = JsonConvert.DeserializeObject<CertificateRequestViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Status))
                        query = query.Where(x => x.Status == search.Status);

                    if (!string.IsNullOrEmpty(search.SubjectDn))
                        query = query.Where(x => x.SubjectDn!.Contains(search.SubjectDn));

                    if (search.CertificateRequestCLMApplicationId.HasValue && search.CertificateRequestCLMApplicationId.Value != Guid.Empty)
                        query = query.Where(x => x.CertificateRequestCLMApplicationId == search.CertificateRequestCLMApplicationId.Value);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<CertificateRequestViewModel> CreateAsync(this ICertificateRequestRepository repository, CertificateRequest entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }
    }
}