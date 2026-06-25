using EcosCLM.Application.ViewModels;
using FluentValidation;

namespace EcosCLM.Application.Validators
{
    public class PolicySettingsValidator : AbstractValidator<PolicySettingsViewModel>
    {
        public PolicySettingsValidator()
        {
        }
    }
}
