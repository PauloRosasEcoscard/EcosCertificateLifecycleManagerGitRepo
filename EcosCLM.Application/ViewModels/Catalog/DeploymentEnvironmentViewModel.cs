using AutoMapper;
using EcosCLM.Application.Validators.Catalog;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Domain.Entities.Catalog;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
namespace EcosCLM.Application.ViewModels.Catalog
{
    public class DeploymentEnvironmentViewModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<DeploymentTargetViewModel> DeploymentTargets { get; set; } = new List<DeploymentTargetViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<DeploymentEnvironmentValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class DeploymentEnvironmentProfile : Profile
    {
        public DeploymentEnvironmentProfile()
        {
            CreateMap<DeploymentEnvironment, DeploymentEnvironmentViewModel>().ReverseMap();
        }
    }
}
