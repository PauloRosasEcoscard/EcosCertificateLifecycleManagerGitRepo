using AutoMapper;
using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
namespace EcosCLM.Application.ViewModels.Certificates
{
    public class CaOrderViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RequestId { get; set; }
        public Guid CaId { get; set; }
        public string? ExternalOrderId { get; set; }
        public string? ExternalCertificateId { get; set; }
        public string Status { get; set; } = "CREATED";
        public DateTime? SubmittedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RawResponseRef { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CertificateRequestViewModel? Request { get; set; }
        public virtual CertificateAuthorityViewModel? CertificateAuthority { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<CaOrderValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class CaOrderProfile : Profile
    {
        public CaOrderProfile()
        {
            CreateMap<CaOrder, CaOrderViewModel>().ReverseMap();
        }
    }
}
