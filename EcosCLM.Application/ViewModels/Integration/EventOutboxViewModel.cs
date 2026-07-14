using AutoMapper;
using EcosCLM.Application.Validators.Integration;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Integration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Integration
{
    public class EventOutboxViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string PayloadJson { get; set; } = "{}";
        public string Status { get; set; } = "PENDING";
        public int Retries { get; set; } = 0;
        public DateTime CreatedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<EventOutboxValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }

    public class EventOutboxProfile : Profile
    {
        public EventOutboxProfile()
        {
            CreateMap<EventOutbox, EventOutboxViewModel>().ReverseMap();
        }
    }
}
