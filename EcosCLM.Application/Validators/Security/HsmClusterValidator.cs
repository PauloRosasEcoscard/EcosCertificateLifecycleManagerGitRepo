using EcosCLM.Application.ViewModels.Security;
using FluentValidation;

namespace EcosCLM.Application.Validators.Security
{
    public class HsmClusterValidator : AbstractValidator<HsmClusterViewModel>
    {
        public HsmClusterValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Vendor)
                .MaximumLength(100);

            RuleFor(x => x.Model)
                .MaximumLength(100);

            RuleFor(x => x.PartitionLabel)
                .MaximumLength(150);

            RuleFor(x => x.EndpointRef)
                .MaximumLength(500);

            RuleFor(x => x.FipsLevel)
                .MaximumLength(100);

            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.MetadataJson)
                .NotEmpty();
        }
    }
}
