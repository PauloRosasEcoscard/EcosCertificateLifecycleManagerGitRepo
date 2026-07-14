using EcosCLM.Application.ViewModels.Security;
using FluentValidation;

namespace EcosCLM.Application.Validators.Security
{
    public class CertificateAuthorityValidator : AbstractValidator<CertificateAuthorityViewModel>
    {
        public CertificateAuthorityValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.ProviderType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.BaseUrl)
                .MaximumLength(500);

            RuleFor(x => x.AccountRef)
                .MaximumLength(255);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.MetadataJson)
                .NotEmpty();
        }
    }
}
