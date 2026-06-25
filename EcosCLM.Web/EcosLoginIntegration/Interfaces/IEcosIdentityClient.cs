using EcosCLM.Web.EcosLoginIntegration.Model;

namespace EcosCLM.Web.EcosLoginIntegration.Interfaces
{
    public interface IEcosIdentityClient
    {
        // AuthController
        Task<string?> LoginAsync(LoginViewModel model);

        // PolicySystemUserController
        Task<UserIntegrationDto?> GetUserByEmailAsync(string email);

        // AzureAuthConfigController
        Task<AuthFlowConfigViewModel?> GetAuthConfigAsync(Guid customerId);

        // CustomerController
        Task<CustomerIntegrationDto?> GetCustomerByIdAsync(Guid customerId);
    }
}
