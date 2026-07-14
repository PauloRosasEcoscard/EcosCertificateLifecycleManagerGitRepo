using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation;

namespace EcosCLM.Application.Validators.Certificates
{
    public class CaOrderValidator : AbstractValidator<CaOrderViewModel>
    {
        public CaOrderValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.RequestId)
                .NotEmpty();

            RuleFor(x => x.CaId)
                .NotEmpty();

            RuleFor(x => x.ExternalOrderId)
                .MaximumLength(255);

            RuleFor(x => x.ExternalCertificateId)
                .MaximumLength(255);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.ErrorCode)
                .MaximumLength(100);

            RuleFor(x => x.ErrorMessage)
                .MaximumLength(1000);

            RuleFor(x => x.RawResponseRef)
                .MaximumLength(500);
        }
    }
}
