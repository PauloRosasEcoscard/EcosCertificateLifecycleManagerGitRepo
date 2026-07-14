using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation;

namespace EcosCLM.Application.Validators.Certificates
{
    public class CertificateRequestSanDnsValidator : AbstractValidator<CertificateRequestSanDnsViewModel>
    {
        public CertificateRequestSanDnsValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.RequestId)
                .NotEmpty();

            RuleFor(x => x.DnsName)
                .NotEmpty()
                .MaximumLength(255)
                .Matches(@"^(?:\*\.)?[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(?:\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$")
                .WithMessage("Invalid SAN DNS format.");
        }
    }
}
