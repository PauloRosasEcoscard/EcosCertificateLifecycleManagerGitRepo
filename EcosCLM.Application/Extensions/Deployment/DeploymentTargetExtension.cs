using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Domain.Entities.Deployment;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Deployment
{
    public static class DeploymentTargetExtension
    {
        public static async Task<DeploymentTargetViewModel> GetByIdAsync(this IDeploymentTargetRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(DeploymentTarget), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<DeploymentTargetViewModel>> GetAllWithPageAsync(this IDeploymentTargetRepository repository, int page = 0, int offset = 0, string? filter = null, string? oderBy = null, string? orderDirection = null, Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

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

                    case "type":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.TargetType) : query.OrderBy(i => i.TargetType);
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
                var search = JsonConvert.DeserializeObject<DeploymentTargetViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Name))
                        query = query.Where(x => x.Name.Contains(search.Name));

                    if (!string.IsNullOrEmpty(search.TargetType))
                        query = query.Where(x => x.TargetType == search.TargetType);

                    if (!string.IsNullOrEmpty(search.Status))
                        query = query.Where(x => x.Status == search.Status);

                    if (search.ApplicationId.HasValue && search.ApplicationId.Value != Guid.Empty)
                        query = query.Where(x => x.ApplicationId == search.ApplicationId.Value);

                    if (search.EnvironmentId.HasValue && search.EnvironmentId.Value != Guid.Empty)
                        query = query.Where(x => x.EnvironmentId == search.EnvironmentId.Value);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<DeploymentTargetViewModel> CreateAsync(this IDeploymentTargetRepository repository, DeploymentTarget entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }
    }
}