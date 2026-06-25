using AutoMapper;
using EcosCLM.Application.Validators;
using EcosCLM.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels
{
    public class PolicySettingsViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustumerId { get; set; }
        public int TimeoutSession { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new PolicySettingsValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }

    public class PolicySettingsProfile : Profile
    {
        public PolicySettingsProfile()
        {
            CreateMap<PolicySettings, PolicySettingsViewModel>().ReverseMap();
        }
    }
}
