using AutoMapper;
using EcosCLM.Application.Validators.Security;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;
using EcosCLM.Domain.Entities.Security;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Security
{
    public class HsmClusterViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Vendor { get; set; }
        public string? Model { get; set; }
        public string? PartitionLabel { get; set; }
        public string? EndpointRef { get; set; }
        public string? FipsLevel { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public string MetadataJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<HsmKeyRefViewModel> Keys { get; set; } = new List<HsmKeyRefViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<HsmClusterValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class HsmClusterProfile : Profile
    {
        public HsmClusterProfile()
        {
            CreateMap<HsmCluster, HsmClusterViewModel>().ReverseMap();
        }
    }
}
