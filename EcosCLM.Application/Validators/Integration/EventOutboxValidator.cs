using EcosCLM.Application.ViewModels.Integration;
using FluentValidation;

namespace EcosCLM.Application.Validators.Integration
{
    public class EventOutboxValidator : AbstractValidator<EventOutboxViewModel>
    {
        public EventOutboxValidator()
        {
            RuleFor(x => x.EventType)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.PayloadJson)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Retries)
                .GreaterThanOrEqualTo(0);
        }
    }
}
