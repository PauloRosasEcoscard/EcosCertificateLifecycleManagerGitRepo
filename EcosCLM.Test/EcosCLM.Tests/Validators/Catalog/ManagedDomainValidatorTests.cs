using EcosCLM.Application.Validators.Catalog;
using EcosCLM.Application.ViewModels.Catalog;
using FluentValidation.TestHelper;

namespace EcosCLM.Tests.Validators.Catalog
{
    public class ManagedDomainValidatorTests
    {
        private readonly ManagedDomainValidator _validator;

        public ManagedDomainValidatorTests()
        {
            _validator = new ManagedDomainValidator();
        }

        [Fact]
        public void Should_Have_Error_When_CustomerId_Is_Empty()
        {
            var model = new ManagedDomainViewModel { CustomerId = Guid.Empty };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Theory]
        [InlineData("ecosclm.com.br")]
        [InlineData("api.ecosclm.com.br")]
        [InlineData("*.ecosclm.com")]
        [InlineData("sub-domain.domain.io")]
        public void Should_Not_Have_Error_When_Fqdn_Is_Valid(string validFqdn)
        {
            var model = new ManagedDomainViewModel { Fqdn = validFqdn };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Fqdn);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid_domain")]
        [InlineData("http://ecosclm.com")]
        [InlineData("ecosclm.com/path")]
        [InlineData("*.sub.*.ecosclm.com")]
        public void Should_Have_Error_When_Fqdn_Is_Invalid(string invalidFqdn)
        {
            var model = new ManagedDomainViewModel { Fqdn = invalidFqdn };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Fqdn)
                .WithErrorMessage("Invalid FQDN format.");
        }
    }
}