using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.Extensions
{
    public static class PolicySettingsExtension
    {
        public static PolicySettingsViewModel GetById(this IPolicySettingsRepository repository, Guid id, Guid customerId)
        {
            var entity = repository?.GetAll()
                .Where(x => x.Id == id)
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefault();

            if (entity == null)
                throw new NotFoundException(nameof(PolicySettings), id);

            return repository.ToViewModel(entity);
        }

        public static PolicySettingsViewModel GetByIdCustumer(this IPolicySettingsRepository repository, Guid customerId)
        {
            var entity = repository?.GetAll()
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefault();

            if (entity == null)
                return null;

            return repository.ToViewModel(entity);
        }


        public static List<PolicySettingsViewModel> GetAllWithPage(this IPolicySettingsRepository repository, Guid? Customer = null, int page = 0, int offset = 0, string filter = null)
        {
            var query = repository?.GetAll();

            query = query.Where(x => x.CustumerId == Customer);

            if (page > 0)
                query = query.Take(page);

            if (offset > 0)
                query = query.Skip(offset);

            return repository.ToListViewModel(query.ToList());
        }

        public static PolicySettingsViewModel Create(this IPolicySettingsRepository repository, PolicySettingsViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = repository.Add(entity);
            return repository.ToViewModel(query);
        }

        public static PolicySettingsViewModel Edit(this IPolicySettingsRepository repository, PolicySettingsViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = repository.Upd(entity);
            return repository.ToViewModel(query);
        }
    }
}
