using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Application.Extensions.Security
{
    public static class CertificateAuthorityExtension
    {
        public static async Task<CertificateAuthorityViewModel> GetByIdAsync(this ICertificateAuthorityRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(CertificateAuthority), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<CertificateAuthorityViewModel> GetByIdAsync(this ICertificateAuthorityRepository repository, Guid id, Guid customerId)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .Where(x => x.CustomerId == customerId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(CertificateAuthority), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<CertificateAuthorityViewModel>> GetAllWithPageAsync(this ICertificateAuthorityRepository repository, int page = 0, int offset = 0, string? filter = null, string? oderBy = null, Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            if (customer.HasValue && customer.Value != Guid.Empty)
            {
                query = query.Where(x => x.CustomerId == customer.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(x => x.Name.Contains(filter) || x.ProviderType.Contains(filter));
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<CertificateAuthorityViewModel> CreateAsync(this ICertificateAuthorityRepository repository, CertificateAuthorityViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<CertificateAuthorityViewModel> CreateAsync(this ICertificateAuthorityRepository repository, CertificateAuthority entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<CertificateAuthorityViewModel> EditAsync(this ICertificateAuthorityRepository repository, CertificateAuthorityViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.UpdAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<bool> DeleteAsync(this ICertificateAuthorityRepository repository, Guid id)
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