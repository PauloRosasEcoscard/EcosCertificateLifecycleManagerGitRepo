using AutoMapper;
using EcosCLM.Application.Validators.Security;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;
using EcosCLM.Domain.Entities.Security;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Security
{
    public class CertificateAuthorityViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProviderType { get; set; } = string.Empty;
        public string? BaseUrl { get; set; }
        public string? AccountRef { get; set; }
        public short SupportsAcme { get; set; } = 0;
        public string Status { get; set; } = "ACTIVE";
        public string MetadataJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CertificateAuthorityValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class CertificateAuthorityProfile : Profile
    {
        public CertificateAuthorityProfile()
        {
            CreateMap<CertificateAuthority, CertificateAuthorityViewModel>().ReverseMap();
        }
    }
}
