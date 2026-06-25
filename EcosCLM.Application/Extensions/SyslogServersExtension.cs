using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.Extensions
{
    public static class SyslogServersExtension
    {
        public static SyslogServersViewModel GetById(this ISyslogServersRepository repository, int id, Guid customerId)
        {
            var entity = repository?.GetAll()
                .Where(x => x.Id == id)
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefault();

            if (entity == null)
                throw new NotFoundException(nameof(SyslogServers), id);

            return repository.ToViewModel(entity);
        }

        public static SyslogServersViewModel GetByIdCustumer(this ISyslogServersRepository repository, Guid customerId)
        {
            var entity = repository?.GetAll()
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefault();

            if (entity == null)
                throw new NotFoundException(nameof(SyslogServers), customerId);

            return repository.ToViewModel(entity);
        }


        public static List<SyslogServersViewModel> GetAllWithPage(this ISyslogServersRepository repository, Guid? Customer = null, int page = 0, int offset = 0, string filter = null)
        {
            var query = repository?.GetAll();

            query = query.Where(x => x.CustumerId == Customer);

            if (page > 0)
                query = query.Take(page);

            if (offset > 0)
                query = query.Skip(offset);

            return repository.ToListViewModel(query.ToList());
        }

        public static SyslogServersViewModel Create(this ISyslogServersRepository repository, SyslogServersViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = repository.Add(entity);
            return repository.ToViewModel(query);
        }
        public static SyslogServersViewModel Edit(this ISyslogServersRepository repository, SyslogServersViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = repository.Upd(entity);
            return repository.ToViewModel(query);
        }

        public static bool Delete(this ISyslogServersRepository repository, int id)
        {
            var entity = repository.FindOne(x => x.Id == id);
            repository.Del(entity);
            return true;
        }
    }
}
