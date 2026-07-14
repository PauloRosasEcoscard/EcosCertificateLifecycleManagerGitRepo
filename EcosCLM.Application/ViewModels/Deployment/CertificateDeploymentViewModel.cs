using AutoMapper;
using EcosCLM.Application.Validators.Deployment;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Deployment;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Deployment
{
    public class CertificateDeploymentViewModel : IValidatableObject 
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CertificateId { get; set; }
        public Guid TargetId { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime? DeployedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CertificateViewModel? Certificate { get; set; }
        public virtual DeploymentTargetViewModel? Target { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CertificateDeploymentValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class CertificateDeploymentProfile : Profile
    {
        public CertificateDeploymentProfile()
        {
            CreateMap<CertificateDeployment, CertificateDeploymentViewModel>().ReverseMap();
        }
    }
}
