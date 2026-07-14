using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Deployment;

namespace EcosCLM.Data.Repositories
{
    public class CertificateDeploymentRepository : BaseRepository<CertificateDeployment, CertificateDeploymentViewModel>, ICertificateDeploymentRepository
    {
        public CertificateDeploymentRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}