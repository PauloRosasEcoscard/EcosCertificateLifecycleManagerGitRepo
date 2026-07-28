using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Application.Extensions.Security
{
    public static class HsmKeyRefExtension
    {
        public static async Task<HsmKeyRefViewModel> GetByIdAsync(this IHsmKeyRefRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(HsmKeyRef), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<HsmKeyRefViewModel> GetByIdAsync(this IHsmKeyRefRepository repository, Guid id, Guid customerId)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .Where(x => x.CustomerId == customerId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(HsmKeyRef), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<HsmKeyRefViewModel>> GetAllWithPageAsync(this IHsmKeyRefRepository repository, int page = 0, int offset = 0, string? filter = null, string? oderBy = null, Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            if (customer.HasValue && customer.Value != Guid.Empty)
            {
                query = query.Where(x => x.CustomerId == customer.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(x => x.KeyLabel.Contains(filter) || x.KeyHandle.Contains(filter) || x.Algorithm.Contains(filter));
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<HsmKeyRefViewModel> CreateAsync(this IHsmKeyRefRepository repository, HsmKeyRefViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<HsmKeyRefViewModel> CreateAsync(this IHsmKeyRefRepository repository, HsmKeyRef entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<HsmKeyRefViewModel> EditAsync(this IHsmKeyRefRepository repository, HsmKeyRefViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.UpdAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<bool> DeleteAsync(this IHsmKeyRefRepository repository, Guid id)
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