using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Domain.Entities.Certificates;

namespace EcosCLM.Application.Interfaces
{
    public interface ICertificateRequestSanDnsRepository : IBaseRepository<CertificateRequestSanDns, CertificateRequestSanDnsViewModel>
    {
    }
}