using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Security
{
    public static class CertificateProfileExtension
    {
        public static async Task<CertificateProfileViewModel> GetByIdAsync(this ICertificateProfileRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(CertificateProfile), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<CertificateProfileViewModel> GetByIdAsync(this ICertificateProfileRepository repository, Guid id, Guid customerId)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .Where(x => x.CustomerId == customerId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(CertificateProfile), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<CertificateProfileViewModel>> GetAllWithPageAsync(
                                                                        this ICertificateProfileRepository repository,
                                                                        int page = 0,
                                                                        int offset = 0,
                                                                        string? filter = null,
                                                                        string? oderBy = null,
                                                                        string? orderDirection = null,
                                                                        Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            if (customer.HasValue && customer.Value != Guid.Empty)
            {
                query = query.Where(x => x.CustomerId == customer.Value);
            }

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "name":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name);
                        break;

                    case "type":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.CertificateType) : query.OrderBy(i => i.CertificateType);
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

            if (!string.IsNullOrWhiteSpace(filter))
            {
                try
                {
                    var search = JsonConvert.DeserializeObject<CertificateProfileViewModel>(filter);

                    if (search != null)
                    {
                        if (!string.IsNullOrWhiteSpace(search.Name))
                            query = query.Where(x => x.Name.Contains(search.Name));

                        if (!string.IsNullOrWhiteSpace(search.CertificateType))
                            query = query.Where(x => x.CertificateType.Contains(search.CertificateType));

                        if (!string.IsNullOrWhiteSpace(search.Status))
                            query = query.Where(x => x.Status == search.Status);
                    }
                }
                catch (JsonException)
                {
                    query = query.Where(x => x.Name.Contains(filter) || x.CertificateType.Contains(filter));
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<CertificateProfileViewModel> CreateAsync(this ICertificateProfileRepository repository, CertificateProfileViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<CertificateProfileViewModel> CreateAsync(this ICertificateProfileRepository repository, CertificateProfile entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<CertificateProfileViewModel> EditAsync(this ICertificateProfileRepository repository, CertificateProfileViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.UpdAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<bool> DeleteAsync(this ICertificateProfileRepository repository, Guid id)
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