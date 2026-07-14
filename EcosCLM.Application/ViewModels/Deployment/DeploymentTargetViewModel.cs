using AutoMapper;
using EcosCLM.Application.Validators.Deployment;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Deployment;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Deployment
{
    public class DeploymentTargetViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? ApplicationId { get; set; }
        public Guid? EnvironmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string? EndpointRef { get; set; }
        public string? SecretRef { get; set; }
        public short AutomationEnabled { get; set; } = 0;
        public string Status { get; set; } = "ACTIVE";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<DeploymentTargetValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class DeploymentTargetProfile : Profile
    {
        public DeploymentTargetProfile()
        {
            CreateMap<DeploymentTarget, DeploymentTargetViewModel>().ReverseMap();
        }
    }
}
