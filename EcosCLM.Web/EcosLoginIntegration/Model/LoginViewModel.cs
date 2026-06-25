using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class LoginViewModel : IValidatableObject
    {
        public string TxUsername { get; set; }
        public string TxPassword { get; set; }
        public string ReturnUrl { get; set; }
        public string VerificationCode { get; set; }
        public bool LembrarMe { get; set; }

        public IEnumerable<ValidationResult> Validate(global::System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new LoginValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }

    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.TxUsername).NotEmpty().WithMessage("O Username deve ser informado");
            RuleFor(x => x.TxPassword).NotEmpty().WithMessage("A Senha deve ser informada");
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
