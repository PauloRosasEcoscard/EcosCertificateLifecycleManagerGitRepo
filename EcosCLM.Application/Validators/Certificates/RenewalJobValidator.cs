using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation;

namespace EcosCLM.Application.Validators.Certificates
{
    public class RenewalJobValidator : AbstractValidator<RenewalJobViewModel>
    {
        public RenewalJobValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.CertificateId)
                .NotEmpty();

            RuleFor(x => x.ScheduledAt)
                .NotEmpty();

            RuleFor(x => x.DueAt)
                .NotEmpty()
                .GreaterThan(x => x.ScheduledAt)
                .WithMessage("Due date (DueAt) must be greater than scheduled date (ScheduledAt).");

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
