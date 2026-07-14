using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Catalog
{
    public static class DeploymentEnvironmentExtension
    {
        public static async Task<DeploymentEnvironmentViewModel> GetByIdAsync(this IDeploymentEnvironmentRepository repository, Guid id)
        {
            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(DeploymentEnvironment), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<DeploymentEnvironmentViewModel>> GetAllWithPageAsync(this IDeploymentEnvironmentRepository repository, int page = 0, int offset = 0, string filter = null, string oderBy = null, string orderDirection = null, Guid? customer = null)
        {
            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "name":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name);
                        break;
                    case "code":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Code) : query.OrderBy(i => i.Code);
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
                var search = JsonConvert.DeserializeObject<DeploymentEnvironmentViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Name))
                        query = query.Where(x => x.Name.Contains(search.Name));

                    if (!string.IsNullOrEmpty(search.Code))
                        query = query.Where(x => x.Code.Contains(search.Code));
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync();
            return repository.ToListViewModel(list);
        }

        public static async Task<DeploymentEnvironmentViewModel> CreateAsync(this IDeploymentEnvironmentRepository repository, DeploymentEnvironment entity)
        {
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }
    }
}