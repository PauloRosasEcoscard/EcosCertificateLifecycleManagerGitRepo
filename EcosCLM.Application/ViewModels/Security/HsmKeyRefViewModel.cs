using AutoMapper;
using EcosCLM.Application.Validators.Security;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;
using EcosCLM.Domain.Entities.Security;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Security
{
    public class HsmKeyRefViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid HsmClusterId { get; set; }
        public string KeyLabel { get; set; } = string.Empty;
        public string KeyHandle { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
        public int? KeySize { get; set; }
        public string? CurveName { get; set; }
        public short Extractable { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual HsmClusterViewModel? HsmCluster { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<HsmKeyRefValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class HsmKeyRefProfile : Profile
    {
        public HsmKeyRefProfile()
        {
            CreateMap<HsmKeyRef, HsmKeyRefViewModel>().ReverseMap();
        }
    }
}