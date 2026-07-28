using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcosCLM.Application.Extensions.Certificates
{
    public static class CertificateRequestSanDnsExtension
    {
        public static async Task<CertificateRequestSanDnsViewModel> GetByIdAsync(this ICertificateRequestSanDnsRepository repository, Guid id)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var entity = await repository.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (entity == null)
                throw new NotFoundException(nameof(CertificateRequestSanDns), id);

            return repository.ToViewModel(entity);
        }

        public static async Task<List<CertificateRequestSanDnsViewModel>> GetAllWithPageAsync(this ICertificateRequestSanDnsRepository repository, int page = 0, int offset = 0, string? filter = null, string? oderBy = null, string? orderDirection = null, Guid? customer = null)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var query = repository.GetAll();

            if (customer.HasValue)
                query = query.Where(x => x.CustomerId == customer.Value);

            query = query.OrderBy(x => x.DnsName);

            if (!string.IsNullOrEmpty(filter))
            {
                var search = JsonConvert.DeserializeObject<CertificateRequestSanDnsViewModel>(filter);

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.DnsName))
                        query = query.Where(x => x.DnsName!.Contains(search.DnsName));

                    if (search.RequestId != Guid.Empty)
                        query = query.Where(x => x.RequestId == search.RequestId);
                }
            }

            if (offset > 0)
                query = query.Skip(offset);

            if (page > 0)
                query = query.Take(page);

            var list = await query.ToListAsync().ConfigureAwait(false);
            return repository.ToListViewModel(list);
        }

        public static async Task<CertificateRequestSanDnsViewModel> CreateAsync(this ICertificateRequestSanDnsRepository repository, CertificateRequestSanDns entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<CertificateRequestSanDnsViewModel> EditAsync(this ICertificateRequestSanDnsRepository repository, CertificateRequestSanDnsViewModel model)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.UpdAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<bool> DeleteAsync(this ICertificateRequestSanDnsRepository repository, Guid id)
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