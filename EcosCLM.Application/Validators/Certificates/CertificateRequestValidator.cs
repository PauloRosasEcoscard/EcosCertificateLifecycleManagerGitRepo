using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation;

namespace EcosCLM.Application.Validators.Certificates
{
    public class CertificateRequestValidator : AbstractValidator<CertificateRequestViewModel>
    {
        public CertificateRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.RequestType)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.SubjectDn)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.KeyPolicyJson)
                .NotEmpty();

            RuleFor(x => x.FailureReason)
                .MaximumLength(1000);

            RuleFor(x => x.MetadataJson)
                .NotEmpty();
        }
    }
}
