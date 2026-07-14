using AutoMapper;
using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Certificates
{
    public class CertificateRequestSanDnsViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RequestId { get; set; }
        public string DnsName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public virtual CertificateRequestViewModel? Request { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CertificateRequestSanDnsValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class CertificateRequestSanDnsProfile : Profile
    {
        public CertificateRequestSanDnsProfile()
        {
            CreateMap<CertificateRequestSanDns, CertificateRequestSanDnsViewModel>().ReverseMap();
        }
    }
}
