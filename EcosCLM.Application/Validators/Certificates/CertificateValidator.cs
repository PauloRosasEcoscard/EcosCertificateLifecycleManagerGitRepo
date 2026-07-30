using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation;

namespace EcosCLM.Application.Validators.Certificates
{
    public class CertificateValidator : AbstractValidator<CertificateViewModel>
    {
        public CertificateValidator()
        {
           

            RuleFor(x => x.SerialNumber)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ThumbprintSha256)
                .NotEmpty()
                .Length(64);

            RuleFor(x => x.SubjectDn)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.IssuerDn)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.NotBefore)
                .NotEmpty();

            RuleFor(x => x.NotAfter)
                .NotEmpty()
                .GreaterThan(x => x.NotBefore)
                .WithMessage("Expiration date (NotAfter) must be greater than issue date (NotBefore).");

            RuleFor(x => x.CertificatePem)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.RevocationReason)
                .MaximumLength(255);

            RuleFor(x => x.MetadataJson)
                .NotEmpty();
        }
    }
}
