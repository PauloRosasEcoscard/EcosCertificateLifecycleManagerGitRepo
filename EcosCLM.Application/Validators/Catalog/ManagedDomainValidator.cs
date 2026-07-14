using EcosCLM.Application.ViewModels.Catalog;
using FluentValidation;

namespace EcosCLM.Application.Validators.Catalog
{
    public class ManagedDomainValidator : AbstractValidator<ManagedDomainViewModel>
    {
        public ManagedDomainValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Fqdn)
                .NotEmpty()
                .MaximumLength(255)
                .Matches(@"^(?:\*\.)?[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(?:\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$")
                .WithMessage("Invalid FQDN format.");

            RuleFor(x => x.ValidationStatus)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.MetadataJson)
                .NotEmpty();
        }
    }
}
