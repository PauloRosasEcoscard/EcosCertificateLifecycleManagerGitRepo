using EcosCLM.Application.ViewModels.Security;
using FluentValidation;

namespace EcosCLM.Application.Validators.Security
{
    public class CertificateProfileValidator : AbstractValidator<CertificateProfileViewModel>
    {
        public CertificateProfileValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.CertificateType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.KeyAlgorithm)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.CurveName)
                .MaximumLength(50);

            RuleFor(x => x.SignatureAlgorithm)
                .MaximumLength(100);

            RuleFor(x => x.ValidityDays)
                .GreaterThan(0);

            RuleFor(x => x.RenewalWindowDays)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SubjectTemplateJson)
                .NotEmpty();

            RuleFor(x => x.SanPolicyJson)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
