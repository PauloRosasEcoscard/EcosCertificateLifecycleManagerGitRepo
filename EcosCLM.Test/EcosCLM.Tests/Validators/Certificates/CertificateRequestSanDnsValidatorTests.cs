using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation.TestHelper;

namespace EcosCLM.Tests.Validators.Certificates
{
    public class CertificateRequestSanDnsValidatorTests
    {
        private readonly CertificateRequestSanDnsValidator _validator;

        public CertificateRequestSanDnsValidatorTests()
        {
            _validator = new CertificateRequestSanDnsValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Identifiers_Are_Empty()
        {
            var model = new CertificateRequestSanDnsViewModel
            {
                RequestId = Guid.Empty
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.RequestId);
        }

        [Theory]
        [InlineData("ecosclm.com.br")]
        [InlineData("api.ecosclm.com.br")]
        [InlineData("*.ecosclm.com")]
        public void Should_Not_Have_Error_When_DnsName_Is_Valid(string validDns)
        {
            var model = new CertificateRequestSanDnsViewModel { DnsName = validDns };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.DnsName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid_dns_name")]
        [InlineData("http://ecosclm.com")]
        [InlineData("ecosclm.com/path")]
        public void Should_Have_Error_When_DnsName_Is_Invalid(string invalidDns)
        {
            var model = new CertificateRequestSanDnsViewModel { DnsName = invalidDns };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DnsName);
        }
    }
}