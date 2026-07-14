using AutoMapper;
using EcosCLM.Application.Validators.Catalog;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Domain.Entities.Base;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace EcosCLM.Application.ViewModels.Catalog
{
    public class CLMApplicationViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? OwnerUserId { get; set; }
        public string Criticality { get; set; } = "MEDIUM";
        public string Status { get; set; } = "ACTIVE";
        public string MetadataJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<ManagedDomainViewModel> Domains { get; set; } = new List<ManagedDomainViewModel>();
        public virtual ICollection<CertificateRequest> CLMApplicationCertificateRequests { get; set; } = new List<CertificateRequest>();
        public virtual ICollection<DeploymentTargetViewModel> DeploymentTargets { get; set; } = new List<DeploymentTargetViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CLMApplicationValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class AccessLogsProfile : Profile
    {
        public AccessLogsProfile()
        {
            CreateMap<AuditLogs, AuditLogsViewModel>().ReverseMap();
        }
    }
}
