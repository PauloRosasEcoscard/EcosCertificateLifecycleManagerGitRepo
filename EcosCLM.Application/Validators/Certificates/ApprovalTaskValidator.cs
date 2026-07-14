using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation;

namespace EcosCLM.Application.Validators.Certificates
{
    public class ApprovalTaskValidator : AbstractValidator<ApprovalTaskViewModel>
    {
        public ApprovalTaskValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.RequestId)
                .NotEmpty();

            RuleFor(x => x.StepOrder)
                .GreaterThan(0);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.DecisionComment)
                .MaximumLength(1000);
        }
    }
}
