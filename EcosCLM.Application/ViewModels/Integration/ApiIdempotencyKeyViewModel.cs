using AutoMapper;
using EcosCLM.Application.Validators.Integration;
using EcosCLM.Domain.Entities.Integration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Integration
{
    public class ApiIdempotencyKeyViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string ResponseJson { get; set; } = "{}";
        public DateTime ExpiresAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<ApiIdempotencyKeyValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class ApiIdempotencyKeyProfile : Profile
    {
        public ApiIdempotencyKeyProfile()
        {
            CreateMap<ApiIdempotencyKey, ApiIdempotencyKeyViewModel>().ReverseMap();
        }
    }
}
