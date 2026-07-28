using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Certificates
{
    public static class CertificateExtension
    {
        public static async Task<CertificateViewModel> GetByIdAsync(this ICertificateRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(Certificate), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<CertificateViewModel>> GetAllWithPageAsync(this ICertificateRepository repository, int page = 0, int offset = 0, string? filter = null, string? oderBy = null, string? orderDirection = null, Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "expiration":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.NotAfter) : query.OrderBy(i => i.NotAfter);
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
                var search = JsonConvert.DeserializeObject<CertificateViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Status))
                        query = query.Where(x => x.Status == search.Status);

                    if (!string.IsNullOrEmpty(search.SerialNumber))
                        query = query.Where(x => x.SerialNumber == search.SerialNumber);

                    if (!string.IsNullOrEmpty(search.SubjectDn))
                        query = query.Where(x => x.SubjectDn!.Contains(search.SubjectDn));
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<CertificateViewModel> CreateAsync(this ICertificateRepository repository, Certificate entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<CertificateViewModel> EditAsync(this ICertificateRepository repository, CertificateViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.UpdAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<bool> DeleteAsync(this ICertificateRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.FindOneAsync(x => x.Id == id).ConfigureAwait(false);

            if (entity != null)
            {
                await repository.DelAsync(entity).ConfigureAwait(false);
            }

            return true;
        }
    }
}