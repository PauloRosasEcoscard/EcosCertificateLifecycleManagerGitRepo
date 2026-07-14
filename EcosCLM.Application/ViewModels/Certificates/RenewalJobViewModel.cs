using AutoMapper;
using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Certificates
{
    public class RenewalJobViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CertificateId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime DueAt { get; set; }
        public string Status { get; set; } = "SCHEDULED";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CertificateViewModel? Certificate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<RenewalJobValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class RenewalJobProfile : Profile
    {
        public RenewalJobProfile()
        {
            CreateMap<RenewalJob, RenewalJobViewModel>().ReverseMap();
        }
    }
}
