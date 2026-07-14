using EcosCLM.Application.ViewModels.Integration;
using FluentValidation;

namespace EcosCLM.Application.Validators.Integration
{
    public class ApiIdempotencyKeyValidator : AbstractValidator<ApiIdempotencyKeyViewModel>
    {
        public ApiIdempotencyKeyValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Key)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.ResponseJson)
                .NotEmpty();

            RuleFor(x => x.ExpiresAt)
                .NotEmpty();
        }
    }
}
