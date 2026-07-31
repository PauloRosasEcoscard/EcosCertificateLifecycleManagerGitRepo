using AutoMapper;
using EcosCLM.Application.Validators.Security;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Domain.Entities.Integration;
using EcosCLM.Domain.Entities.Security;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Security
{
    public class CertificateProfileViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CertificateType { get; set; } = string.Empty;
        public string KeyAlgorithm { get; set; } = string.Empty;
        public int? KeySize { get; set; }
        public string? CurveName { get; set; }
        public string? SignatureAlgorithm { get; set; }
        public int ValidityDays { get; set; }
        public int RenewalWindowDays { get; set; } = 30;
        public string SubjectTemplateJson { get; set; } = "{}";
        public string SanPolicyJson { get; set; } = "{}";
        public short RequireApproval { get; set; } = 1;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CertificateProfileValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class CertificateProfileProfile : Profile
    {
        public CertificateProfileProfile()
        {
            CreateMap<CertificateProfile, CertificateProfileViewModel>().ReverseMap();
        }
    }
}
