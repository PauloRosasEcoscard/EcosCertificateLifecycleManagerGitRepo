namespace EcosCLM.Domain.DataTypes
{
    public static class PolicySystemUris
    {
        // Identity provider
        public const string getIdentityProviderByCustumer = "/api/AzureAuthConfig/getClientAuth/{0}"; // {0} = CustumerId;
        public const string getIdentityProviderByCustumerIdentityProvider = "/api/AzureAuthConfig/getClientAuth/{0}/{1}"; // {0} = CustumerId, {1} = IdentityProvider ID;
        public const string getClientAuthFlowConfig = "/api/AzureAuthConfig/getClientAuthFlowConfig/{0}"; // {0} = CustumerId;
        public const string addIdentityProvider = "/api/AzureAuthConfig/addClientAuth";
        public const string getAzureGroupRoleMapping = "/api/AzureAuthConfig/getAzureGroupRoleMappings/{0}";
        public const string addAzureGroupRoleMapping = "/api/AzureAuthConfig/addAzureGroupRoleMappings";
        public const string updAzureGroupRoleMappings = "/api/AzureAuthConfig/updateAzureGroupRoleMappings";
        public const string editIdentityProvider = "/api/AzureAuthConfig/editClientAuth";
        public const string deleteIdentityProvider = "/api/AzureAuthConfig/delete/{0}"; // {0} = IdentityProvider ID
        public const string validateAzureCredentials = "/api/AzureAuthConfig/validateAzureCredentials";
        public const string authenticateAzureAPI = "/api/AzureAuthConfig/authenticateAzureAPI";
        public const string validateAzureToken = "/api/AzureAuthConfig/validate";
        public const string interpretProfile = "/api/AzureAuthConfig/interpretProfile";

        // User Management
        public const string getCompanyUsers = "/api/PolicySystemUser/getCompanyUsers/{0}"; // {0} = CustumerId
        public const string existByEmail = "/api/PolicySystemUser/existByEmail/{0}"; // {0} = email
        public const string getByEmail = "/api/PolicySystemUser/getByEmail/{0}"; // {0} = email
        public const string getUser = "/api/PolicySystemUser/get/{0}"; // {0} = User id
        public const string getProfile = "/api/PolicySystemUser/getProfile/{0}"; // {0} = email
        public const string getProfileId = "/api/PolicySystemUser/getProfileId/{0}"; // {0} = email
        public const string addUser = "/api/PolicySystemUser/add";
        public const string editUser = "/api/PolicySystemUser/edit/{0}";
        public const string editUserProfile = "/api/PolicySystemUser/editUserProfile/{0}";
        public const string deleteUser = "/api/PolicySystemUser/delete/{0}"; // {0} = User id
        public const string isBlocked = "/api/PolicySystemUser/isBlocked";
        public const string ValidatePassword = "/Auth/ValidatePassword";
        public const string RegisterNewPassword = "/Auth/RegisterNewPassword";

        // Customer / Client Management
        public const string getAllCustomers = "/api/Customer/getAll?page={0}&offset={1}";
        public const string getAllCustomersWithFilter = "/api/Customer/getAll?page={0}&offset={1}&filter={2}";
        public const string getCustomerById = "/api/Customer/getById/{0}";
        public const string getCustomerByName = "/api/Customer/getByName/{0}";
        public const string createCustomer = "/api/Customer/create";
        public const string editCustomer = "/api/Customer/edit";
        public const string deleteCustomer = "/api/Customer/delete/{0}";

        // PolicySystem Base Endpoints
        public const string validateUserById = "/PolicySystem/ValidateUserById?idUser={0}"; // {0} = idUser
        public const string validateUserByEmailPassword = "/PolicySystem/ValidateUserByEmailPassword";
        public const string getAllUserProfileById = "/PolicySystem/GetAllUserProfileById?idUser={0}"; // {0} = idUser
        public const string getAllUserProfileByEmail = "/PolicySystem/GetAllUserProfileByEmail?email={0}"; // {0} = email
        public const string getUserCustomerId = "/PolicySystem/GetUserCustomerId?email={0}"; // {0} = email
        public const string getUser2faSecret = "/PolicySystem/GetUser2faSecret?email={0}"; // {0} = email
        public const string checkModuleFromProfile = "/PolicySystem/CheckModuleFromProfile?idProfile={0}&idType={1}"; // {0} = idProfile, {1} = idType
        public const string getAllModuleByIdProfile = "/PolicySystem/GetAllModuleByIdProfile?idProfile={0}&idType={1}"; // {0} = idProfile, {1} = idType
        public const string validateApiUserByToken = "/PolicySystem/ValidateApiUserByToken";
        public const string validateByUsername = "/PolicySystem/ValidateByUsername?username={0}"; // {0} = username
        public const string encryptRoles = "/PolicySystem/EncryptRoles";
        public const string decryptRoles = "/PolicySystem/DecryptRoles";

        // Profile Management
        public const string getProfileById = "/api/PolicySystemProfile/get/{0}"; // {0} = Profile Id
        public const string getAllProfilesWithPage = "/api/PolicySystemProfile/getAllByPage?page={0}&offset={1}"; // {0} = page, {1} = offset
        public const string getAllProfilesWithPageAndFilter = "/api/PolicySystemProfile/getAll?page={0}&offset={1}&filter={2}"; // {0} = page, {1} = offset, {2} = filter
        public const string getAllProfilesList = "/api/PolicySystemProfile/getAll";
        public const string addProfile = "/api/PolicySystemProfile/add";
        public const string editProfile = "/api/PolicySystemProfile/edit";
        public const string deleteProfile = "/api/PolicySystemProfile/delete/{0}"; // {0} = Profile Id
    }
}