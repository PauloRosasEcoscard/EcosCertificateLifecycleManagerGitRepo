using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Domain.Entities.Deployment;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Deployment
{
    public static class CertificateDeploymentExtension
    {
        public static async Task<CertificateDeploymentViewModel> GetByIdAsync(this ICertificateDeploymentRepository repository, Guid id)
        {
            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(nameof(CertificateDeployment), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<CertificateDeploymentViewModel>> GetAllWithPageAsync(this ICertificateDeploymentRepository repository, int page = 0, int offset = 0, string filter = null, string oderBy = null, string orderDirection = null, Guid? customer = null)
        {
            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "deployed":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.DeployedAt) : query.OrderBy(i => i.DeployedAt);
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
                var search = JsonConvert.DeserializeObject<CertificateDeploymentViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Status))
                        query = query.Where(x => x.Status == search.Status);

                    if (search.CertificateId != Guid.Empty)
                        query = query.Where(x => x.CertificateId == search.CertificateId);

                    if (search.TargetId != Guid.Empty)
                        query = query.Where(x => x.TargetId == search.TargetId);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync();
            return repository.ToListViewModel(list);
        }

        public static async Task<CertificateDeploymentViewModel> CreateAsync(this ICertificateDeploymentRepository repository, CertificateDeployment entity)
        {
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }
    }
}