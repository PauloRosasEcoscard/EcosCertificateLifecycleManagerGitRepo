using EcosCLM.Application.Validators.Catalog;
using EcosCLM.Application.ViewModels.Catalog;
using FluentValidation.TestHelper;
using Xunit;

namespace EcosCLM.Tests.Validators.Catalog
{
    public class CLMApplicationValidatorTests
    {
        private readonly CLMApplicationValidator _validator;

        public CLMApplicationValidatorTests()
        {
            _validator = new CLMApplicationValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Code_Is_Empty()
        {
            var model = new CLMApplicationViewModel
            {
                CustomerId = Guid.NewGuid(),
                Code = string.Empty,
                Name = "Valid Name",
                Criticality = "LOW"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = new CLMApplicationViewModel
            {
                CustomerId = Guid.NewGuid(),
                Code = "APP01",
                Name = new string('A', 151),
                Criticality = "LOW"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData("LOW")]
        [InlineData("MEDIUM")]
        [InlineData("HIGH")]
        public void Should_Not_Have_Error_When_Criticality_Is_Valid(string criticality)
        {
            var model = new CLMApplicationViewModel
            {
                CustomerId = Guid.NewGuid(),
                Code = "APP01",
                Name = "Valid Name",
                Criticality = criticality
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Criticality);
        }

        [Theory]
        [InlineData("URGENT")]
        [InlineData("INVALID_VALUE")]
        public void Should_Have_Error_When_Criticality_Is_Invalid(string invalidCriticality)
        {
            var model = new CLMApplicationViewModel
            {
                CustomerId = Guid.NewGuid(),
                Code = "APP01",
                Name = "Valid Name",
                Criticality = invalidCriticality
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Criticality);
        }
    }
}