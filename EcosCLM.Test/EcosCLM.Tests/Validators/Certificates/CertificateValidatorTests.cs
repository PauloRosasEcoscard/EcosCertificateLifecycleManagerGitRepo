using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation.TestHelper;

namespace EcosCLM.Tests.Validators.Certificates
{
    public class CertificateValidatorTests
    {
        private readonly CertificateValidator _validator;

        public CertificateValidatorTests()
        {
            _validator = new CertificateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Required_Fields_Are_Empty()
        {
            var model = new CertificateViewModel
            {
                SerialNumber = string.Empty,
                ThumbprintSha256 = string.Empty,
                SubjectDn = string.Empty,
                IssuerDn = string.Empty,
                CertificatePem = string.Empty
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.SerialNumber);
            result.ShouldHaveValidationErrorFor(x => x.ThumbprintSha256);
            result.ShouldHaveValidationErrorFor(x => x.SubjectDn);
            result.ShouldHaveValidationErrorFor(x => x.IssuerDn);
            result.ShouldHaveValidationErrorFor(x => x.CertificatePem);
        }

        [Fact]
        public void Should_Have_Error_When_Thumbprint_Length_Is_Not_64_Characters()
        {
            var model = new CertificateViewModel { ThumbprintSha256 = "SHORT_THUMBPRINT" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ThumbprintSha256);
        }

        [Fact]
        public void Should_Have_Error_When_Expiration_Is_Before_Issue_Date()
        {
            var model = new CertificateViewModel
            {
                NotBefore = DateTime.UtcNow,
                NotAfter = DateTime.UtcNow.AddDays(-1)
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.NotAfter)
                .WithErrorMessage("Expiration date (NotAfter) must be greater than issue date (NotBefore).");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Dates_Are_Valid()
        {
            var model = new CertificateViewModel
            {
                NotBefore = DateTime.UtcNow,
                NotAfter = DateTime.UtcNow.AddYears(1)
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.NotAfter);
        }
    }
}