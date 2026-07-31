using AutoMapper;
using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace EcosCLM.Application.ViewModels.Certificates
{
    public class CertificateRequestViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? CertificateRequestCLMApplicationId { get; set; }
        public Guid? CertificateRequestDomainId { get; set; }
        public Guid? CertificateRequestProfileId { get; set; }
        public Guid? CaId { get; set; }
        public Guid? HsmClusterId { get; set; }
        public Guid? HsmKeyRefId { get; set; }
        public Guid? RequestedBy { get; set; }
        public string SubjectDn { get; set; } = string.Empty;
        public string KeyPolicyJson { get; set; } = "{}";
        public string? CsrPem { get; set; }
        public string? FailureReason { get; set; }
        public string MetadataJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CLMApplicationViewModel? CertificateRequestCLMApplication { get; set; }
        public virtual ManagedDomainViewModel? CertificateRequestDomain { get; set; }
        public virtual CertificateProfileViewModel? CertificateRequestProfile { get; set; }
        public virtual CertificateAuthorityViewModel? CertificateAuthority { get; set; }
        public virtual ICollection<CertificateRequestSanDnsViewModel> SanDns { get; set; } = new List<CertificateRequestSanDnsViewModel>();
        public virtual ICollection<ApprovalTaskViewModel> ApprovalTasks { get; set; } = new List<ApprovalTaskViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CertificateRequestValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class CertificateRequestProfile : Profile
    {
        public CertificateRequestProfile()
        {
            CreateMap<CertificateRequest, CertificateRequestViewModel>().ReverseMap();
        }
    }
}
