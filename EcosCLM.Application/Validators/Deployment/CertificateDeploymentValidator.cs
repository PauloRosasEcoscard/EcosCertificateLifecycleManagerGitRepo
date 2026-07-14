using EcosCLM.Application.ViewModels.Deployment;
using FluentValidation;

namespace EcosCLM.Application.Validators.Deployment
{
    public class CertificateDeploymentValidator : AbstractValidator<CertificateDeploymentViewModel>
    {
        public CertificateDeploymentValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.CertificateId)
                .NotEmpty();

            RuleFor(x => x.TargetId)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.ErrorMessage)
                .MaximumLength(1000);
        }
    }
}
