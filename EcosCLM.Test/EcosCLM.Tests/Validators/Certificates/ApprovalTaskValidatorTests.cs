using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Application.ViewModels.Certificates;
using FluentValidation.TestHelper;

namespace EcosCLM.Tests.Validators.Certificates
{
    public class ApprovalTaskValidatorTests
    {
        private readonly ApprovalTaskValidator _validator;

        public ApprovalTaskValidatorTests()
        {
            _validator = new ApprovalTaskValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Identifiers_Are_Empty()
        {
            var model = new ApprovalTaskViewModel
            {
                RequestId = Guid.Empty,
                CustomerId = Guid.Empty
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.RequestId);
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_StepOrder_Is_Less_Than_One(int invalidStep)
        {
            var model = new ApprovalTaskViewModel { StepOrder = invalidStep };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.StepOrder);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void Should_Not_Have_Error_When_StepOrder_Is_Valid(int validStep)
        {
            var model = new ApprovalTaskViewModel { StepOrder = validStep };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.StepOrder);
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Empty()
        {
            var model = new ApprovalTaskViewModel { Status = string.Empty };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Status);
        }
    }
}