using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Security;

namespace EcosCLM.Data.Repositories
{
    public class CertificateAuthorityRepository : BaseRepository<CertificateAuthority, CertificateAuthorityViewModel>, ICertificateAuthorityRepository
    {
        public CertificateAuthorityRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}