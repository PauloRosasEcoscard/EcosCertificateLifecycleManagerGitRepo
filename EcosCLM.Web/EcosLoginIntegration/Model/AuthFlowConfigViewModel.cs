using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class AuthFlowConfigViewModel
    {
        public int Id { get; set; }
        public Guid IdCustomer { get; set; }
        public bool DefaultAutenthicationEnabled { get; set; }
        public bool MicrosoftAutenthicationEnabled { get; set; }
        public bool MicrosoftIdentityProviderEnabled { get; set; }
        public bool MultFactorAuthenticationEnabled { get; set; }

    }
}
