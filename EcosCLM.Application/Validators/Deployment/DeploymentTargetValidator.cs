using EcosCLM.Application.ViewModels.Deployment;
using FluentValidation;

namespace EcosCLM.Application.Validators.Deployment
{
    public class DeploymentTargetValidator : AbstractValidator<DeploymentTargetViewModel>
    {
        public DeploymentTargetValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.TargetType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.EndpointRef)
                .MaximumLength(500);

            RuleFor(x => x.SecretRef)
                .MaximumLength(500);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
