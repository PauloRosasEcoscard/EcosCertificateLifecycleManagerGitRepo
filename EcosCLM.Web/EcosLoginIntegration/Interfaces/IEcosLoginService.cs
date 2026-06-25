using EcosCLM.Web.EcosLoginIntegration.Model;

namespace EcosCLM.Web.EcosLoginIntegration.Interfaces
{
    public interface IEcosLoginService
    {
        #region Auth Methods
        Task<IntegrationResult<string>> Login(LoginViewModel viewModel);
        Task<IntegrationResult<string>> LoginAzure(LoginViewModel viewModel);
        Task<IntegrationResult<string>> FirstAccess(LoginViewModel viewModel);
        Task<IntegrationResult<string>> RegisterNewPswUser(PolicySystemUserViewModel viewModel);
        Task<IntegrationResult<string>> ForgotPassword(ForgotPasswordViewModel viewModel);
        Task<IntegrationResult<string>> PasswordSetInEmail(ForgotPasswordViewModel viewModel);
        Task<IntegrationResult<string>> GetAsync(string token, object entity);
        Task<IntegrationResult<string>> PostAsync(string token, object entity);
        #endregion

        #region PolicySystem Methods
        Task<IntegrationResult<string>> ValidateUserById(Guid idUser);
        Task<IntegrationResult<string>> ValidateUserByEmailPassword(LoginViewModel viewModel);
        Task<IntegrationResult<string>> GetAllUserProfileById(Guid idUser);
        Task<IntegrationResult<string>> GetAllUserProfileByEmail(string email);
        Task<IntegrationResult<Guid?>> GetUserCustomerId(string email);
        Task<IntegrationResult<string>> GetUser2faSecret(string email);
        Task<IntegrationResult<string>> CheckModuleFromProfile(Guid idProfile, int idType = 1);
        Task<IntegrationResult<string>> GetAllModuleByIdProfile(Guid idProfile, int idType = 1);
        Task<IntegrationResult<MembershipContext>> ValidateApiUserByToken(string token);
        Task<IntegrationResult<string>> ValidateByUsername(string username);
        Task<IntegrationResult<string>> EncryptRoles(object values, string encryptKey);
        Task<IntegrationResult<T>> DecryptRoles<T>(string roles, string encryptKey);
        #endregion

        #region PolicySystemUser Methods
        Task<IntegrationResult<PolicySystemUserViewModel>> AddPolicySystemUser(PolicySystemUserViewModel model);
        Task<IntegrationResult<PolicySystemUserViewModel>> EditPolicySystemUserProfile(Guid id, PolicySystemUserViewModel model);
        Task<IntegrationResult<PolicySystemUserViewModel>> EditPolicySystemUser(Guid id);
        Task<IntegrationResult<bool>> DeletePolicySystemUser(Guid id);
        Task<IntegrationResult<PolicySystemUserViewModel>> GetPolicySystemUserById(Guid id);
        Task<IntegrationResult<PolicySystemUserViewModel>> GetPolicySystemUserByEmail(string email);
        Task<IntegrationResult<PolicySystemUserViewModel>> ExistPolicySystemUserByEmail(string email);
        Task<IntegrationResult<IEnumerable<PolicySystemUserViewModel>>> GetPolicySystemCompanyUsers(Guid customerId);
        Task<IntegrationResult<string>> GetProfile(string email);
        Task<IntegrationResult<Guid?>> GetProfileId(string email);
        Task<IntegrationResult<PolicySystemUserViewModel>> RegisterNewPasswordUser(PolicySystemUserViewModel model, string returnUrl);
        Task<IntegrationResult<bool>> GetUserIsBlocked(PolicySystemUserViewModel model);
        #endregion

        #region Customer Methods
        Task<IntegrationResult<List<PolicySystemCustomerViewModel>>> GetAllCustomers(int page = 0, int offset = 0, string filter = null);
        Task<IntegrationResult<PolicySystemCustomerViewModel>> GetCustomerById(Guid id);
        Task<IntegrationResult<PolicySystemCustomerViewModel>> GetCustomerByName(string name);
        Task<IntegrationResult<PolicySystemCustomerViewModel>> CreateCustomer(PolicySystemCustomerViewModel model);
        Task<IntegrationResult<PolicySystemCustomerViewModel>> EditCustomer(PolicySystemCustomerViewModel model);
        Task<IntegrationResult<bool>> DeleteCustomer(Guid id);
        #endregion

        #region AzureAuthConfig Methods
        Task<IntegrationResult<AuthConfigAzureViewModel>> CreateAuthConfig(AuthConfigAzureViewModel model);
        Task<IntegrationResult<List<AzureGroupRoleMappingViewModel>>> GetAzureGroupRoleMappings(Guid customerId);
        Task<IntegrationResult<List<AzureGroupRoleMappingViewModel>>> CreateAzureGroupRoleMappings(List<AzureGroupRoleMappingViewModel> models);
        Task<IntegrationResult<string>> UpdateAzureGroupRoleMappings(List<AzureGroupRoleMappingViewModel> models);
        Task<IntegrationResult<AuthConfigAzureViewModel>> EditAuthConfig(AuthConfigAzureViewModel model);
        Task<IntegrationResult<AuthFlowConfigViewModel>> GetAuthFlowConfigByCustomerId(Guid customerId);
        Task<IntegrationResult<bool>> EditClientAuthFlowConfig(Guid customerId, AuthFlowConfigViewModel model);
        Task<IntegrationResult<List<AuthConfigAzureViewModel>>> GetAuthConfigByCustomerId(Guid customerId);
        Task<IntegrationResult<List<AuthConfigAzureViewModel>>> GetAuthConfigByCustomerName(string customerName);
        Task<IntegrationResult<AuthConfigAzureViewModel>> GetAuthConfigByClientIdAndId(Guid customerId, int id);
        Task<IntegrationResult<string>> AuthenticateAzureAPI(LoginAzureAPI model);
        Task<IntegrationResult<string>> ValidateAzureCredentials(Azurevalidate model);
        Task<IntegrationResult<string>> ValidateAzureToken(TokenRequestAzure model);
        Task<IntegrationResult<bool>> DeleteAuthConfig(int id);
        Task<IntegrationResult<AzureGroupRoleMappingViewModel>> InterpretProfile(AzureGroupRoleMappingViewModel model);
        #endregion

        #region PolicySystemProfile Methods
        Task<IntegrationResult<PolicySystemProfileViewModel>> GetProfileById(Guid id);
        Task<IntegrationResult<List<PolicySystemProfileViewModel>>> GetAllProfilesWithPage(int page = 0, int offset = 0, string filter = null);
        Task<IntegrationResult<List<PolicySystemProfileViewModel>>> GetAllProfilesList();
        Task<IntegrationResult<PolicySystemProfileViewModel>> CreateProfile(PolicySystemProfileViewModel model);
        Task<IntegrationResult<PolicySystemProfileViewModel>> EditProfile(PolicySystemProfileViewModel model);
        Task<IntegrationResult<bool>> DeleteProfile(Guid id);
        #endregion
    }
}