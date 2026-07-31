using EcosCLM.Application.ViewModels.Security;
using FluentValidation;

namespace EcosCLM.Application.Validators.Security
{
    public class HsmKeyRefValidator : AbstractValidator<HsmKeyRefViewModel>
    {
        public HsmKeyRefValidator()
        {
            RuleFor(x => x.HsmClusterId)
                .NotEmpty();

            RuleFor(x => x.KeyLabel)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.KeyHandle)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Algorithm)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.CurveName)
                .MaximumLength(50);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}