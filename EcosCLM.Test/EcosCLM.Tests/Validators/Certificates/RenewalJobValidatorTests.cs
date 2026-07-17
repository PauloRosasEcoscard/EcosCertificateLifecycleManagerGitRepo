using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation.TestHelper;

namespace EcosCLM.Tests.Validators.Certificates
{
    public class RenewalJobValidatorTests
    {
        private readonly RenewalJobValidator _validator;

        public RenewalJobValidatorTests()
        {
            _validator = new RenewalJobValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Identifiers_Are_Empty()
        {
            var model = new RenewalJobViewModel
            {
                CertificateId = Guid.Empty,
                CustomerId = Guid.Empty
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.CertificateId);
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Empty()
        {
            var model = new RenewalJobViewModel { Status = string.Empty };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new RenewalJobViewModel
            {
                CertificateId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ScheduledAt = DateTime.UtcNow.AddDays(30),
                DueAt = DateTime.UtcNow.AddDays(45),
                Status = "SCHEDULED"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.CertificateId);
            result.ShouldNotHaveValidationErrorFor(x => x.CustomerId);
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }
    }
}