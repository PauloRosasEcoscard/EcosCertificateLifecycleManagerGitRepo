using AutoMapper;
using EcosCLM.Application.Validators.Catalog;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Catalog;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace EcosCLM.Application.ViewModels.Catalog
{
    public class ManagedDomainViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? ApplicationId { get; set; }
        public string Fqdn { get; set; } = string.Empty;
        public string ValidationStatus { get; set; } = "PENDING";
        public DateTime? ValidatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string MetadataJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CLMApplicationViewModel? Application { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<ManagedDomainValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class ManagedDomainProfile : Profile
    {
        public ManagedDomainProfile()
        {
            CreateMap<ManagedDomain, ManagedDomainViewModel>().ReverseMap();
        }
    }
}
