using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Application.Extensions.Base
{
    public static class PolicySettingsExtension
    {
        public static async Task<PolicySettingsViewModel> GetByIdAsync(this IPolicySettingsRepository repository, Guid id, Guid customerId)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(PolicySettings), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<PolicySettingsViewModel?> GetByIdCustomerAsync(this IPolicySettingsRepository repository, Guid customerId)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                return null;

            return repository.ToViewModel(entity);
        }

        public static async Task<List<PolicySettingsViewModel>> GetAllWithPageAsync(this IPolicySettingsRepository repository, Guid? Customer = null, int page = 0, int offset = 0, string? filter = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            query = query.Where(x => x.CustumerId == Customer);

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<PolicySettingsViewModel> CreateAsync(this IPolicySettingsRepository repository, PolicySettingsViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<PolicySettingsViewModel> EditAsync(this IPolicySettingsRepository repository, PolicySettingsViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.UpdAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }
    }
}