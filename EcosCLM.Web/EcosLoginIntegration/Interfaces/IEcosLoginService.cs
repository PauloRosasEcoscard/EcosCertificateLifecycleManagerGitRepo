using EcosCLM.Web.EcosLoginIntegration.Model;

namespace EcosCLM.Web.EcosLoginIntegration.Interfaces
{
    public interface IEcosLoginService
    {
        #region Auth Methods
        public Task<IntegrationResult<string>> Login(LoginViewModel viewModel);
        public Task<IntegrationResult<string>> LoginAzure(LoginViewModel viewModel);
        public Task<IntegrationResult<string>> FirstAccess(LoginViewModel viewModel);
        public Task<IntegrationResult<string>> RegisterNewPswUser(PolicySystemUserViewModel viewModel);
        public Task<IntegrationResult<string>> ForgotPassword(ForgotPasswordViewModel viewModel);
        public Task<IntegrationResult<string>> PasswordSetInEmail(ForgotPasswordViewModel viewModel);
        public Task<IntegrationResult<string>> GetAsync(string token, object entity);
        public Task<IntegrationResult<string>> PostAsync(string token, object entity);
        #endregion

        #region PolicySystem Methods
        public Task<IntegrationResult<string>> ValidateUserById(Guid idUser);
        public Task<IntegrationResult<string>> ValidateUserByEmailPassword(LoginViewModel viewModel);
        public Task<IntegrationResult<string>> GetAllUserProfileById(Guid idUser);
        public Task<IntegrationResult<string>> GetAllUserProfileByEmail(string email);
        public Task<IntegrationResult<Guid?>> GetUserCustomerId(string email);
        public Task<IntegrationResult<string>> GetUser2faSecret(string email);
        public Task<IntegrationResult<string>> CheckModuleFromProfile(Guid idProfile, int idType = 1);
        public Task<IntegrationResult<string>> GetAllModuleByIdProfile(Guid idProfile, int idType = 1);
        public Task<IntegrationResult<MembershipContext>> ValidateApiUserByToken(string token);
        public Task<IntegrationResult<string>> ValidateByUsername(string username);
        public Task<IntegrationResult<string>> EncryptRoles(object values, string encryptKey);
        public Task<IntegrationResult<T>> DecryptRoles<T>(string roles, string encryptKey);
        #endregion

        #region PolicySystemUser Methods
        public Task<IntegrationResult<PolicySystemUserViewModel>> AddPolicySystemUser(PolicySystemUserViewModel model);
        public Task<IntegrationResult<PolicySystemUserViewModel>> EditPolicySystemUserProfile(Guid id, PolicySystemUserViewModel model);
        public Task<IntegrationResult<PolicySystemUserViewModel>> EditPolicySystemUser(Guid id);
        public Task<IntegrationResult<bool>> DeletePolicySystemUser(Guid id);
        public Task<IntegrationResult<PolicySystemUserViewModel>> GetPolicySystemUserById(Guid id);
        public Task<IntegrationResult<PolicySystemUserViewModel>> GetPolicySystemUserByEmail(string email);
        public Task<IntegrationResult<PolicySystemUserViewModel>> ExistPolicySystemUserByEmail(string email);
        public Task<IntegrationResult<IEnumerable<PolicySystemUserViewModel>>> GetPolicySystemCompanyUsers(Guid customerId);
        public Task<IntegrationResult<string>> GetProfile(string email);
        public Task<IntegrationResult<Guid?>> GetProfileId(string email);
        public Task<IntegrationResult<PolicySystemUserViewModel>> RegisterNewPasswordUser(PolicySystemUserViewModel model, string returnUrl);
        public Task<IntegrationResult<bool>> GetUserIsBlocked(PolicySystemUserViewModel model);
        #endregion

        #region Customer Methods
        public Task<IntegrationResult<List<PolicySystemCustomerViewModel>>> GetAllCustomers(int page = 0, int offset = 0, string filter = null);
        public Task<IntegrationResult<PolicySystemCustomerViewModel>> GetCustomerById(Guid id);
        public Task<IntegrationResult<PolicySystemCustomerViewModel>> GetCustomerByName(string name);
        public Task<IntegrationResult<PolicySystemCustomerViewModel>> CreateCustomer(PolicySystemCustomerViewModel model);
        public Task<IntegrationResult<PolicySystemCustomerViewModel>> EditCustomer(PolicySystemCustomerViewModel model);
        public Task<IntegrationResult<bool>> DeleteCustomer(Guid id);
        #endregion

        #region AzureAuthConfig Methods
        public Task<IntegrationResult<AuthConfigAzureViewModel>> CreateAuthConfig(AuthConfigAzureViewModel model);
        public Task<IntegrationResult<List<AzureGroupRoleMappingViewModel>>> GetAzureGroupRoleMappings(Guid customerId);
        public Task<IntegrationResult<List<AzureGroupRoleMappingViewModel>>> CreateAzureGroupRoleMappings(List<AzureGroupRoleMappingViewModel> models);
        public Task<IntegrationResult<string>> UpdateAzureGroupRoleMappings(List<AzureGroupRoleMappingViewModel> models);
        public Task<IntegrationResult<AuthConfigAzureViewModel>> EditAuthConfig(AuthConfigAzureViewModel model);
        public Task<IntegrationResult<AuthFlowConfigViewModel>> GetAuthFlowConfigByCustomerId(Guid customerId);
        public Task<IntegrationResult<bool>> EditClientAuthFlowConfig(Guid customerId, AuthFlowConfigViewModel model);
        public Task<IntegrationResult<List<AuthConfigAzureViewModel>>> GetAuthConfigByCustomerId(Guid customerId);
        public Task<IntegrationResult<List<AuthConfigAzureViewModel>>> GetAuthConfigByCustomerName(string customerName);
        public Task<IntegrationResult<AuthConfigAzureViewModel>> GetAuthConfigByClientIdAndId(Guid customerId, int id);
        public Task<IntegrationResult<string>> AuthenticateAzureAPI(LoginAzureAPI model);
        public Task<IntegrationResult<string>> ValidateAzureCredentials(Azurevalidate model);
        public Task<IntegrationResult<string>> ValidateAzureToken(TokenRequestAzure model);
        public Task<IntegrationResult<bool>> DeleteAuthConfig(int id);
        public Task<IntegrationResult<AzureGroupRoleMappingViewModel>> InterpretProfile(AzureGroupRoleMappingViewModel model);
        #endregion

        #region PolicySystemProfile Methods
        public Task<IntegrationResult<PolicySystemProfileViewModel>> GetProfileById(Guid id);
        public Task<IntegrationResult<List<PolicySystemProfileViewModel>>> GetAllProfilesWithPage(int page = 0, int offset = 0, string filter = null);
        public Task<IntegrationResult<List<PolicySystemProfileViewModel>>> GetAllProfilesList();
        public Task<IntegrationResult<PolicySystemProfileViewModel>> CreateProfile(PolicySystemProfileViewModel model);
        public Task<IntegrationResult<PolicySystemProfileViewModel>> EditProfile(PolicySystemProfileViewModel model);
        public Task<IntegrationResult<bool>> DeleteProfile(Guid id);
        #endregion
    }
}