using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Integration
{
    public static class EventOutboxExtension
    {
        public static async Task<EventOutboxViewModel> GetByIdAsync(this IEventOutboxRepository repository, Guid id)
        {
            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(EventOutbox), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<EventOutboxViewModel>> GetAllWithPageAsync(this IEventOutboxRepository repository, int page = 0, int offset = 0, string filter = null, string oderBy = null, string orderDirection = null, Guid? customer = null)
        {
            var query = repository.GetAll();

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "type":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.EventType) : query.OrderBy(i => i.EventType);
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
                var search = JsonConvert.DeserializeObject<EventOutboxViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Status))
                        query = query.Where(x => x.Status == search.Status);

                    if (!string.IsNullOrEmpty(search.EventType))
                        query = query.Where(x => x.EventType.Contains(search.EventType));
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync();
            return repository.ToListViewModel(list);
        }

        public static async Task<EventOutboxViewModel> CreateAsync(this IEventOutboxRepository repository, EventOutbox entity)
        {
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }
    }
}