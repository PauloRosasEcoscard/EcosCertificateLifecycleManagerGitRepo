using AutoMapper;
using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace EcosCLM.Application.ViewModels.Certificates
{
    public class CertificateViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? ApplicationId { get; set; }
        public Guid? DomainId { get; set; }
        public Guid? CaId { get; set; }
        public Guid? HsmKeyRefId { get; set; }
        public Guid? PreviousCertificateId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string ThumbprintSha256 { get; set; } = string.Empty;
        public string SubjectDn { get; set; } = string.Empty;
        public string IssuerDn { get; set; } = string.Empty;
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        public string CertificatePem { get; set; } = string.Empty;
        public string? ChainPem { get; set; }
        public string Status { get; set; } = "ISSUED";
        public string? RevocationReason { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime? InstalledAt { get; set; }
        public string MetadataJson { get; set; } = "{}";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CertificateRequestViewModel? Request { get; set; }
        public virtual CertificateViewModel? PreviousCertificate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CertificateValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class CertificateProfile : Profile
    {
        public CertificateProfile()
        {
            CreateMap<Certificate, CertificateViewModel>().ReverseMap();
        }
    }
}
