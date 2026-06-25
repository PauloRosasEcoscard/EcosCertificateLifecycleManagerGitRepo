using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class ForgotPasswordViewModel
    {
        public string TxEmail { get; set; }
        public string TxPassword { get; set; }
        public bool LembrarMe { get; set; }
        public string? ReturnUrl { get; set; }

    }
}
