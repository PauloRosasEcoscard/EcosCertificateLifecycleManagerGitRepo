using EcosCLM.Application.ViewModels.Catalog;
using FluentValidation;

namespace EcosCLM.Application.Validators.Catalog
{
    public class CLMApplicationValidator : AbstractValidator<CLMApplicationViewModel>
    {
        public CLMApplicationValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Criticality)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.MetadataJson)
                .NotEmpty();
        }
    }
}