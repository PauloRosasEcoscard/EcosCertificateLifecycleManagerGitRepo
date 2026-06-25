using System.Security.Principal;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class MembershipContext
    {
        public IPrincipal Principal { get; set; }
        public ErrorViewModel Error { get; set; }
        public ContextUserViewModel User { get; set; }
        public ContextApiUserViewModel ApiUserClient { get; set; }
        public List<ContextRolesViewModel> Roles { get; set; }

        public bool IsValido()
        {
            return Principal != null;
        }
    }
}
