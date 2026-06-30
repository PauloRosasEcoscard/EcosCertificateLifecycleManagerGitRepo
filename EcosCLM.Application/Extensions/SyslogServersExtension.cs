using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Application.Extensions
{
    public static class SyslogServersExtension
    {
        public static async Task<SyslogServersViewModel> GetByIdAsync(this ISyslogServersRepository repository, int id, Guid customerId)
        {
            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(SyslogServers), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<SyslogServersViewModel> GetByIdCustomerAsync(this ISyslogServersRepository repository, Guid customerId)
        {
            var entity = await repository.GetAll()
                .Where(x => x.CustumerId == customerId)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(SyslogServers), customerId);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<SyslogServersViewModel>> GetAllWithPageAsync(this ISyslogServersRepository repository, Guid? Customer = null, int page = 0, int offset = 0, string filter = null)
        {
            var query = repository.GetAll();

            query = query.Where(x => x.CustumerId == Customer);

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync();
            return repository.ToListViewModel(list);
        }

        public static async Task<SyslogServersViewModel> CreateAsync(this ISyslogServersRepository repository, SyslogServersViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }

        public static async Task<SyslogServersViewModel> EditAsync(this ISyslogServersRepository repository, SyslogServersViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = await repository.UpdAsync(entity);
            return repository.ToViewModel(query);
        }

        public static async Task<bool> DeleteAsync(this ISyslogServersRepository repository, int id)
        {
            var entity = await repository.FindOneAsync(x => x.Id == id);

            if (entity != null)
            {
                await repository.DelAsync(entity);
            }

            return true;
        }
    }
}