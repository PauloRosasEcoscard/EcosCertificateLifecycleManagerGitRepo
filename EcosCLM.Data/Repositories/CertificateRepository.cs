using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Certificates;

namespace EcosCLM.Data.Repositories
{
    public class CertificateRepository : BaseRepository<Certificate, CertificateViewModel>, ICertificateRepository
    {
        public CertificateRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}