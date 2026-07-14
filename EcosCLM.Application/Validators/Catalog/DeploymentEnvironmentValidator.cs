using EcosCLM.Application.ViewModels.Catalog;
using FluentValidation;

namespace EcosCLM.Application.Validators.Catalog
{
    public class DeploymentEnvironmentValidator : AbstractValidator<DeploymentEnvironmentViewModel>
    {
        public DeploymentEnvironmentValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Description)
                .MaximumLength(500);
        }
    }
}
