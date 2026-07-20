using EcosCLM.Web.EcosLoginIntegration.Model;

namespace EcosCLM.Web.EcosLoginIntegration.Interfaces
{
    public interface IEcosIdentityClient
    {
        // AuthController
        public Task<string?> LoginAsync(LoginViewModel model);

        // PolicySystemUserController
        public Task<UserIntegrationDto?> GetUserByEmailAsync(string email);

        // AzureAuthConfigController
        public Task<AuthFlowConfigViewModel?> GetAuthConfigAsync(Guid customerId);

        // CustomerController
        public Task<CustomerIntegrationDto?> GetCustomerByIdAsync(Guid customerId);
    }
}
