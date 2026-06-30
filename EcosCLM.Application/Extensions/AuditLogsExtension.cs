using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions
{
    public static class AuditLogsExtension
    {
        public static async Task<AuditLogsViewModel> GetByIdAsync(this IAuditLogsRepository repository, Guid id)
        {
            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(AuditLogs), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<AuditLogsViewModel>> GetAllWithPageAsync(this IAuditLogsRepository repository, int page = 0, int offset = 0, string filter = null, string oderBy = null, string orderDirection = null, Guid? Customer = null)
        {
            var query = repository.GetAll();

            query = query.Where(x => x.IdCustumer == Customer);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "date":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Date) : query.OrderBy(i => i.Date);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.Date);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var search = JsonConvert.DeserializeObject<AuditLogsViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.User))
                        query = query.Where(x => x.User.Contains(search.User));

                    if (search.IdCustumer != Guid.Empty)
                        query = query.Where(x => x.IdCustumer == search.IdCustumer);

                    if (search.SearchStartDate != null)
                        query = query.Where(x => x.Date > search.SearchStartDate);

                    if (search.SearchEndDate != null)
                        query = query.Where(x => x.Date < search.SearchEndDate);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync();
            return repository.ToListViewModel(list);
        }

        public static async Task<AuditLogsViewModel> CreateAsync(this IAuditLogsRepository repository, AuditLogs entity)
        {
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }
    }
}