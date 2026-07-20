using AutoMapper;
using EcosCLM.Domain.DataTypes;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using Newtonsoft.Json;
using RestSharp;

namespace EcosCLM.Web.EcosLoginIntegration.Services
{
    public class EcosLoginService : IEcosLoginService
    {
        private string BaseURL { get; set; }
        private readonly IConfiguration _config;
        private readonly ILogger<IEcosLoginService> _logger;
        private readonly IMapper _mapper;

        public EcosLoginService(IConfiguration config,
                                ILogger<IEcosLoginService> logger,
                                IMapper mapper)
        {
            _config = config;
            _logger = logger;
            BaseURL = _config["AppSettings:Clients:Login"];
            _mapper = mapper;
        }

        #region Auth Methods
        public async Task<IntegrationResult<string>> Login(LoginViewModel viewModel)
        {
            _logger.LogInformation("Starting login attempt for user: {Username}", viewModel.TxUsername);
            try
            {
                string baseSiteUrl = $"{BaseURL}/auth/Login";
                LoginModel login = new LoginModel
                {
                    Username = viewModel.TxUsername,
                    Password = viewModel.TxPassword,
                    ReturnUrl = viewModel.ReturnUrl
                };
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(login);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Login failed for user: {Username}. Status Code: {StatusCode}", viewModel.TxUsername, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Login failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user: {Username}", viewModel.TxUsername);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> LoginAzure(LoginViewModel viewModel)
        {
            _logger.LogInformation("Starting Azure login attempt for user: {Username}", viewModel.TxUsername);
            try
            {
                string baseSiteUrl = $"{BaseURL}/auth/LoginAzure";
                LoginModel login = new LoginModel
                {
                    Username = viewModel.TxUsername,
                    ReturnUrl = viewModel.ReturnUrl
                };
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(login);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Azure login failed for user: {Username}. Status Code: {StatusCode}", viewModel.TxUsername, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Azure login failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Azure login for user: {Username}", viewModel.TxUsername);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> FirstAccess(LoginViewModel viewModel)
        {
            _logger.LogInformation("Checking first access for user: {Username}", viewModel.TxUsername);
            try
            {
                string baseSiteUrl = $"{BaseURL}/auth/FirstAccess";
                LoginModel login = new LoginModel
                {
                    Username = viewModel.TxUsername,
                    ReturnUrl = viewModel.ReturnUrl
                };
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(login);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("First access check failed for user: {Username}. Status Code: {StatusCode}", viewModel.TxUsername, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "First access validation failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking first access for user: {Username}", viewModel.TxUsername);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> RegisterNewPswUser(PolicySystemUserViewModel viewModel)
        {
            _logger.LogInformation("Registering new password for user ID: {UserId}, Email: {Email}", viewModel.IdUser, viewModel.TxEmail);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.RegisterNewPassword}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(viewModel);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to register new password for user: {Email}. Status Code: {StatusCode}", viewModel.TxEmail, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Failed to register password");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new password for user: {Email}", viewModel.TxEmail);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            _logger.LogInformation("Requesting forgot password link for: {Email}", viewModel.TxEmail);
            try
            {
                string baseSiteUrl = $"{BaseURL}/auth/CreateFirstPasswordToEmail";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(viewModel);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Forgot password request failed for: {Email}. Status Code: {StatusCode}", viewModel.TxEmail, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Forgot password request failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on forgot password request for: {Email}", viewModel.TxEmail);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> PasswordSetInEmail(ForgotPasswordViewModel viewModel)
        {
            _logger.LogInformation("Sending password email notification to: {Email}", viewModel.TxEmail);
            try
            {
                string baseSiteUrl = $"{BaseURL}/auth/SendEmail";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(viewModel);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to send password email to: {Email}. Status Code: {StatusCode}", viewModel.TxEmail, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Failed to send email");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password email to: {Email}", viewModel.TxEmail);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> GetAsync(string token, object entity)
        {
            _logger.LogInformation("Executing generic GET request to auth/Login.");
            try
            {
                string baseSiteUrl = $"{BaseURL}/auth/Login";
                var client = new RestClient(baseSiteUrl);
                client.AddDefaultHeader("Authorization", $"Bearer {token}");
                var request = new RestRequest().AddJsonBody(entity);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Generic GET request failed. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Generic GET failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing generic GET request.");
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> PostAsync(string token, object entity)
        {
            _logger.LogInformation("Executing generic POST request to auth/Login.");
            try
            {
                string baseSiteUrl = $"{BaseURL}/auth/Login";
                var client = new RestClient(baseSiteUrl);
                client.AddDefaultHeader("Authorization", $"Bearer {token}");
                var request = new RestRequest().AddJsonBody(entity);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Generic POST request failed. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Generic POST failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing generic POST request.");
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }
        #endregion

        #region PolicySystem Methods
        public async Task<IntegrationResult<string>> ValidateUserById(Guid idUser)
        {
            _logger.LogInformation("Requesting user validation for ID: {UserId}", idUser);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.validateUserById, idUser)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("User validation failed for ID: {UserId}. Status Code: {StatusCode}", idUser, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "User validation failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user by ID: {UserId}", idUser);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> ValidateUserByEmailPassword(LoginViewModel viewModel)
        {
            _logger.LogInformation("Requesting validation by email: {Email}", viewModel.TxUsername);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.validateUserByEmailPassword}";
                LoginModel login = new LoginModel
                {
                    Username = viewModel.TxUsername,
                    Password = viewModel.TxPassword,
                    ReturnUrl = viewModel.ReturnUrl
                };
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(login);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Validation by email/password failed for: {Email}. Status Code: {StatusCode}", viewModel.TxUsername, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Email/Password validation failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user by email/password for: {Email}", viewModel.TxUsername);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> GetAllUserProfileById(Guid idUser)
        {
            _logger.LogInformation("Requesting all user profiles for ID: {UserId}", idUser);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getAllUserProfileById, idUser)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to get profiles for user ID: {UserId}. Status Code: {StatusCode}", idUser, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profiles retrieval failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profiles for user ID: {UserId}", idUser);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> GetAllUserProfileByEmail(string email)
        {
            _logger.LogInformation("Requesting all user profiles for email: {Email}", email);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getAllUserProfileByEmail, email)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to get profiles for email: {Email}. Status Code: {StatusCode}", email, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profiles retrieval failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profiles for email: {Email}", email);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<Guid?>> GetUserCustomerId(string email)
        {
            _logger.LogInformation("Requesting Customer ID for email: {Email}", email);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getUserCustomerId, email)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get Customer ID for email: {Email}. Status Code: {StatusCode}", email, response.StatusCode);
                    return IntegrationResult<Guid?>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Customer ID retrieval failed");
                }

                var result = JsonConvert.DeserializeAnonymousType(response.Content, new { idCustomer = Guid.Empty });
                return IntegrationResult<Guid?>.Success(result?.idCustomer, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Customer ID for email: {Email}", email);
                return IntegrationResult<Guid?>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> GetUser2faSecret(string email)
        {
            _logger.LogInformation("Requesting 2FA Secret for email: {Email}", email);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getUser2faSecret, email)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get 2FA Secret for email: {Email}. Status Code: {StatusCode}", email, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "2FA Secret retrieval failed");
                }

                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(response.Content);
                    if (doc.RootElement.TryGetProperty("secret", out var secretElement))
                    {
                        string cleanSecret = secretElement.GetString() ?? string.Empty;
                        return IntegrationResult<string>.Success(cleanSecret, (int)response.StatusCode);
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    // Se falhar e não for JSON, assume que o conteúdo retornado já era a string pura
                }

                return IntegrationResult<string>.Success(response.Content, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting 2FA Secret for email: {Email}", email);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> CheckModuleFromProfile(Guid idProfile, int idType = 1)
        {
            _logger.LogInformation("Checking module access for Profile: {ProfileId}, Type: {Type}", idProfile, idType);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.checkModuleFromProfile, idProfile, idType)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed checking module access for Profile: {ProfileId}. Status Code: {StatusCode}", idProfile, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Module access verification failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking module access for Profile: {ProfileId}", idProfile);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> GetAllModuleByIdProfile(Guid idProfile, int idType = 1)
        {
            _logger.LogInformation("Requesting modules for Profile: {ProfileId}, Type: {Type}", idProfile, idType);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getAllModuleByIdProfile, idProfile, idType)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed getting modules for Profile: {ProfileId}. Status Code: {StatusCode}", idProfile, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Modules retrieval failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting modules for Profile: {ProfileId}", idProfile);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<MembershipContext>> ValidateApiUserByToken(string token)
        {
            _logger.LogInformation("Validating API User by token via body.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.validateApiUserByToken}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(JsonConvert.SerializeObject(token));

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Token validation request failed. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<MembershipContext>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Token validation failed");
                }

                var data = JsonConvert.DeserializeObject<MembershipContext>(response.Content);
                return IntegrationResult<MembershipContext>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating API user by token.");
                return IntegrationResult<MembershipContext>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> ValidateByUsername(string username)
        {
            _logger.LogInformation("Validating access by username: {Username}", username);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.validateByUsername, username)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Validation failed for username: {Username}. Status Code: {StatusCode}", username, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Username validation failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating by username: {Username}", username);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> EncryptRoles(object values, string encryptKey)
        {
            _logger.LogInformation("Sending request to encrypt roles data.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.encryptRoles}";
                var payload = new { Values = values, EncryptKey = encryptKey };

                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(payload);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to encrypt roles. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Roles encryption failed");
                }

                var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                string encryptedRoles = result?.encryptedRoles ?? string.Empty;
                return IntegrationResult<string>.Success(encryptedRoles, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting roles via integrated login service.");
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<T>> DecryptRoles<T>(string roles, string encryptKey)
        {
            _logger.LogInformation("Sending request to decrypt roles data.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.decryptRoles}";
                var payload = new { Roles = roles, EncryptKey = encryptKey };

                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(payload);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to decrypt roles. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<T>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Roles decryption failed");
                }

                var data = JsonConvert.DeserializeObject<T>(response.Content);
                return IntegrationResult<T>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting roles via integrated login service.");
                return IntegrationResult<T>.Failure(500, ex.Message);
            }
        }
        #endregion

        #region PolicySystemUser Methods
        public async Task<IntegrationResult<PolicySystemUserViewModel>> AddPolicySystemUser(PolicySystemUserViewModel model)
        {
            _logger.LogInformation("Sending request to add new PolicySystemUser: {Email}", model.TxEmail);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.addUser}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);
                request.Method = Method.Post;
                request.AddJsonBody(model);

                var response = await client.ExecuteAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to add user: {Email}. Status Code: {StatusCode}", model.TxEmail, response.StatusCode);
                    return IntegrationResult<PolicySystemUserViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Failed to add user");
                }

                var viewModel = JsonConvert.DeserializeObject<PolicySystemUserViewModel>(response.Content);
                return IntegrationResult<PolicySystemUserViewModel>.Success(viewModel, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding PolicySystemUser: {Email}", model.TxEmail);
                return IntegrationResult<PolicySystemUserViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemUserViewModel>> EditPolicySystemUserProfile(Guid id, PolicySystemUserViewModel model)
        {
            _logger.LogInformation("Sending request to edit user profile for User ID: {UserId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.editUserProfile, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PutAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to edit user profile for ID: {UserId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<PolicySystemUserViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Failed to edit user profile");
                }

                var viewModel = JsonConvert.DeserializeObject<PolicySystemUserViewModel>(response.Content);
                return IntegrationResult<PolicySystemUserViewModel>.Success(viewModel, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing user profile for ID: {UserId}", id);
                return IntegrationResult<PolicySystemUserViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemUserViewModel>> EditPolicySystemUser(Guid id)
        {
            _logger.LogInformation("Sending request to edit user for User ID: {UserId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.editUser, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.PutAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to edit user for ID: {UserId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<PolicySystemUserViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Failed to edit user");
                }

                var viewModel = JsonConvert.DeserializeObject<PolicySystemUserViewModel>(response.Content);
                return IntegrationResult<PolicySystemUserViewModel>.Success(viewModel, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing user for ID: {UserId}", id);
                return IntegrationResult<PolicySystemUserViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<bool>> DeletePolicySystemUser(Guid id)
        {
            _logger.LogInformation("Sending request to delete user ID: {UserId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.deleteUser, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.DeleteAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to delete user ID: {UserId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<bool>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Failed to delete user");
                }

                return IntegrationResult<bool>.Success(true, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user ID: {UserId}", id);
                return IntegrationResult<bool>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemUserViewModel>> GetPolicySystemUserById(Guid id)
        {
            _logger.LogInformation("Sending request to get user by ID: {UserId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getUser, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get user by ID: {UserId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<PolicySystemUserViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "User retrieval failed");
                }

                var viewModel = JsonConvert.DeserializeObject<PolicySystemUserViewModel>(response.Content);
                return IntegrationResult<PolicySystemUserViewModel>.Success(viewModel, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                return IntegrationResult<PolicySystemUserViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemUserViewModel>> GetPolicySystemUserByEmail(string email)
        {
            _logger.LogInformation("Sending request to get user by email: {Email}", email);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getByEmail, email)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get user by email: {Email}. Status Code: {StatusCode}", email, response.StatusCode);
                    return IntegrationResult<PolicySystemUserViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "User retrieval failed");
                }

                var viewModel = JsonConvert.DeserializeObject<PolicySystemUserViewModel>(response.Content);
                return IntegrationResult<PolicySystemUserViewModel>.Success(viewModel, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return IntegrationResult<PolicySystemUserViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemUserViewModel>> ExistPolicySystemUserByEmail(string email)
        {
            _logger.LogInformation("Sending request to check if user exists by email: {Email}", email);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.existByEmail, email)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Check existence failed for email: {Email}. Status Code: {StatusCode}", email, response.StatusCode);
                    return IntegrationResult<PolicySystemUserViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Existence validation failed");
                }

                var viewModel = JsonConvert.DeserializeObject<PolicySystemUserViewModel>(response.Content);
                return IntegrationResult<PolicySystemUserViewModel>.Success(viewModel, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence for email: {Email}", email);
                return IntegrationResult<PolicySystemUserViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<IEnumerable<PolicySystemUserViewModel>>> GetPolicySystemCompanyUsers(Guid customerId)
        {
            _logger.LogInformation("Sending request to get company users for Customer ID: {CustomerId}", customerId);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getCompanyUsers, customerId)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get company users for Customer ID: {CustomerId}. Status Code: {StatusCode}", customerId, response.StatusCode);
                    return IntegrationResult<IEnumerable<PolicySystemUserViewModel>>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Company users retrieval failed");
                }

                var viewModels = JsonConvert.DeserializeObject<IEnumerable<PolicySystemUserViewModel>>(response.Content) ?? Enumerable.Empty<PolicySystemUserViewModel>();
                return IntegrationResult<IEnumerable<PolicySystemUserViewModel>>.Success(viewModels, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company users for Customer ID: {CustomerId}", customerId);
                return IntegrationResult<IEnumerable<PolicySystemUserViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> GetProfile(string email)
        {
            _logger.LogInformation("Sending request to get profile title for email: {Email}", email);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getProfile, email)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to get profile title for email: {Email}. Status Code: {StatusCode}", email, response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profile title retrieval failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile title for email: {Email}", email);
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<Guid?>> GetProfileId(string email)
        {
            _logger.LogInformation("Sending request to get profile ID for email: {Email}", email);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getProfileId, email)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get profile ID for email: {Email}. Status Code: {StatusCode}", email, response.StatusCode);
                    return IntegrationResult<Guid?>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profile ID retrieval failed");
                }

                if (Guid.TryParse(response.Content.Replace("\"", ""), out Guid profileId))
                {
                    return IntegrationResult<Guid?>.Success(profileId, (int)response.StatusCode);
                }

                return IntegrationResult<Guid?>.Failure(400, "Invalid Guid format from response content");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile ID for email: {Email}", email);
                return IntegrationResult<Guid?>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemUserViewModel>> RegisterNewPasswordUser(PolicySystemUserViewModel model, string returnUrl)
        {
            _logger.LogInformation("Sending request to register new password for User ID: {UserId}", model.IdUser);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.RegisterNewPassword}";

                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to register new password for User ID: {UserId}. Status Code: {StatusCode}", model.IdUser, response.StatusCode);
                    return IntegrationResult<PolicySystemUserViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Password registration failed");
                }

                var viewModel = JsonConvert.DeserializeObject<PolicySystemUserViewModel>(response.Content);
                return IntegrationResult<PolicySystemUserViewModel>.Success(viewModel, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new password for User ID: {UserId}", model.IdUser);
                return IntegrationResult<PolicySystemUserViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<bool>> GetUserIsBlocked(PolicySystemUserViewModel model)
        {
            _logger.LogInformation("Sending request to check if user is blocked for email: {Email}", model?.TxEmail);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.isBlocked}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to check if user is blocked. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<bool>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Block verification failed");
                }

                var result = JsonConvert.DeserializeAnonymousType(response.Content, new { isBlocked = false });
                bool isBlockedValue = result?.isBlocked ?? false;
                return IntegrationResult<bool>.Success(isBlockedValue, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user is blocked for email: {Email}", model?.TxEmail);
                return IntegrationResult<bool>.Failure(500, ex.Message);
            }
        }
        #endregion

        #region Customer Methods
        public async Task<IntegrationResult<List<PolicySystemCustomerViewModel>>> GetAllCustomers(int page = 0, int offset = 0, string filter = null)
        {
            _logger.LogInformation("Retrieving customers list. Page: {Page}, Offset: {Offset}, Filter: {Filter}", page, offset, filter);
            try
            {
                string baseSiteUrl = string.IsNullOrEmpty(filter)
                    ? $"{BaseURL}{string.Format(PolicySystemUris.getAllCustomers, page, offset)}"
                    : $"{BaseURL}{string.Format(PolicySystemUris.getAllCustomersWithFilter, page, offset, Uri.EscapeDataString(filter))}";

                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to retrieve customers list. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<List<PolicySystemCustomerViewModel>>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Customers list retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<List<PolicySystemCustomerViewModel>>(response.Content);
                return IntegrationResult<List<PolicySystemCustomerViewModel>>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers list from integrated login service.");
                return IntegrationResult<List<PolicySystemCustomerViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemCustomerViewModel>> GetCustomerById(Guid id)
        {
            _logger.LogInformation("Retrieving customer details for ID: {CustomerId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getCustomerById, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to retrieve customer {CustomerId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<PolicySystemCustomerViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Customer retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<PolicySystemCustomerViewModel>(response.Content);
                return IntegrationResult<PolicySystemCustomerViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerId} from integrated login service.", id);
                return IntegrationResult<PolicySystemCustomerViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemCustomerViewModel>> GetCustomerByName(string name)
        {
            _logger.LogInformation("Retrieving customer details for ID: {CustomerId}", name);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getCustomerByName, name)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to retrieve customer {CustomerId}. Status Code: {StatusCode}", name, response.StatusCode);
                    return IntegrationResult<PolicySystemCustomerViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Customer retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<PolicySystemCustomerViewModel>(response.Content);
                return IntegrationResult<PolicySystemCustomerViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerId} from integrated login service.", name);
                return IntegrationResult<PolicySystemCustomerViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemCustomerViewModel>> CreateCustomer(PolicySystemCustomerViewModel model)
        {
            _logger.LogInformation("Sending request to create a new customer.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.createCustomer}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to create customer. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<PolicySystemCustomerViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Customer creation failed");
                }

                var data = JsonConvert.DeserializeObject<PolicySystemCustomerViewModel>(response.Content);
                return IntegrationResult<PolicySystemCustomerViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer via integrated login service.");
                return IntegrationResult<PolicySystemCustomerViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemCustomerViewModel>> EditCustomer(PolicySystemCustomerViewModel model)
        {
            _logger.LogInformation("Sending request to update customer ID: {CustomerId}", model?.IdCustomer);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.editCustomer}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PutAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to update customer ID: {CustomerId}. Status Code: {StatusCode}", model?.IdCustomer, response.StatusCode);
                    return IntegrationResult<PolicySystemCustomerViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Customer update failed");
                }

                var data = JsonConvert.DeserializeObject<PolicySystemCustomerViewModel>(response.Content);
                return IntegrationResult<PolicySystemCustomerViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer ID: {CustomerId} via integrated login service.", model?.IdCustomer);
                return IntegrationResult<PolicySystemCustomerViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<bool>> DeleteCustomer(Guid id)
        {
            _logger.LogInformation("Sending request to delete customer ID: {CustomerId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.deleteCustomer, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.DeleteAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to delete customer ID: {CustomerId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<bool>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Customer deletion failed");
                }

                return IntegrationResult<bool>.Success(true, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer ID: {CustomerId} via integrated login service.", id);
                return IntegrationResult<bool>.Failure(500, ex.Message);
            }
        }
        #endregion

        #region AzureAuthConfig Methods
        public async Task<IntegrationResult<AuthConfigAzureViewModel>> CreateAuthConfig(AuthConfigAzureViewModel model)
        {
            _logger.LogInformation("Sending request to create AuthConfig Azure.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.addIdentityProvider}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to create AuthConfig Azure. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<AuthConfigAzureViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Azure AuthConfig creation failed");
                }

                var data = JsonConvert.DeserializeObject<AuthConfigAzureViewModel>(response.Content);
                return IntegrationResult<AuthConfigAzureViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating AuthConfig Azure via integrated login service.");
                return IntegrationResult<AuthConfigAzureViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<List<AzureGroupRoleMappingViewModel>>> GetAzureGroupRoleMappings(Guid customerId)
        {
            _logger.LogInformation("Requesting Azure group role mappings for Customer ID: {CustomerId}", customerId);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getAzureGroupRoleMapping, customerId)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get Azure group role mappings for Customer ID: {CustomerId}. Status Code: {StatusCode}", customerId, response.StatusCode);
                    return IntegrationResult<List<AzureGroupRoleMappingViewModel>>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Role mappings retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<List<AzureGroupRoleMappingViewModel>>(response.Content);
                return IntegrationResult<List<AzureGroupRoleMappingViewModel>>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Azure group role mappings for Customer ID: {CustomerId}", customerId);
                return IntegrationResult<List<AzureGroupRoleMappingViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<List<AzureGroupRoleMappingViewModel>>> CreateAzureGroupRoleMappings(List<AzureGroupRoleMappingViewModel> models)
        {
            _logger.LogInformation("Sending request to create multiple Azure group role mappings.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.addAzureGroupRoleMapping}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(models);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to create Azure group role mappings. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<List<AzureGroupRoleMappingViewModel>>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Mappings creation failed");
                }

                var data = JsonConvert.DeserializeObject<List<AzureGroupRoleMappingViewModel>>(response.Content);
                return IntegrationResult<List<AzureGroupRoleMappingViewModel>>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Azure group role mappings via integrated login service.");
                return IntegrationResult<List<AzureGroupRoleMappingViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> UpdateAzureGroupRoleMappings(List<AzureGroupRoleMappingViewModel> models)
        {
            _logger.LogInformation("Sending request to update Azure group role mappings.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.updAzureGroupRoleMappings}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(models);

                var response = await client.PutAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to update Azure group role mappings. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Mappings update failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Azure group role mappings via integrated login service.");
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<AuthConfigAzureViewModel>> EditAuthConfig(AuthConfigAzureViewModel model)
        {
            _logger.LogInformation("Sending request to edit AuthConfig Azure.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.editIdentityProvider}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to edit AuthConfig Azure. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<AuthConfigAzureViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "AuthConfig edit failed");
                }

                var data = JsonConvert.DeserializeObject<AuthConfigAzureViewModel>(response.Content);
                return IntegrationResult<AuthConfigAzureViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing AuthConfig Azure via integrated login service.");
                return IntegrationResult<AuthConfigAzureViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<AuthFlowConfigViewModel>> GetAuthFlowConfigByCustomerId(Guid customerId)
        {
            _logger.LogInformation("Requesting AuthFlowConfig for Customer ID: {CustomerId}", customerId);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getClientAuthFlowConfig, customerId)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get AuthFlowConfig for Customer ID: {CustomerId}. Status Code: {StatusCode}", customerId, response.StatusCode);
                    return IntegrationResult<AuthFlowConfigViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "AuthFlowConfig retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<AuthFlowConfigViewModel>(response.Content);
                return IntegrationResult<AuthFlowConfigViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AuthFlowConfig for Customer ID: {CustomerId}", customerId);
                return IntegrationResult<AuthFlowConfigViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<bool>> EditClientAuthFlowConfig(Guid customerId, AuthFlowConfigViewModel model)
        {
            _logger.LogInformation("Sending request to edit AuthFlowConfig for Customer ID: {CustomerId}", customerId);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getClientAuthFlowConfig, customerId)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to edit AuthFlowConfig for Customer ID: {CustomerId}. Status Code: {StatusCode}", customerId, response.StatusCode);
                    return IntegrationResult<bool>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "AuthFlowConfig edit failed");
                }

                return IntegrationResult<bool>.Success(true, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing AuthFlowConfig for Customer ID: {CustomerId}", customerId);
                return IntegrationResult<bool>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<List<AuthConfigAzureViewModel>>> GetAuthConfigByCustomerId(Guid customerId)
        {
            _logger.LogInformation("Requesting AuthConfigs list for Customer ID: {CustomerId}", customerId);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getIdentityProviderByCustumer, customerId)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get AuthConfigs for Customer ID: {CustomerId}. Status Code: {StatusCode}", customerId, response.StatusCode);
                    return IntegrationResult<List<AuthConfigAzureViewModel>>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "AuthConfigs retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<List<AuthConfigAzureViewModel>>(response.Content);
                return IntegrationResult<List<AuthConfigAzureViewModel>>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AuthConfigs for Customer ID: {CustomerId}", customerId);
                return IntegrationResult<List<AuthConfigAzureViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<List<AuthConfigAzureViewModel>>> GetAuthConfigByCustomerName(string customerName)
        {
            _logger.LogInformation("Requesting AuthConfigs list for Customer ID: {CustomerId}", customerName);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getCustomerByName, customerName)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get AuthConfigs for Customer ID: {CustomerId}. Status Code: {StatusCode}", customerName, response.StatusCode);
                    return IntegrationResult<List<AuthConfigAzureViewModel>>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "AuthConfigs retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<List<AuthConfigAzureViewModel>>(response.Content);
                return IntegrationResult<List<AuthConfigAzureViewModel>>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AuthConfigs for Customer ID: {CustomerId}", customerName);
                return IntegrationResult<List<AuthConfigAzureViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<AuthConfigAzureViewModel>> GetAuthConfigByClientIdAndId(Guid customerId, int id)
        {
            _logger.LogInformation("Requesting specific AuthConfig ID: {Id} for Customer ID: {CustomerId}", id, customerId);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getIdentityProviderByCustumerIdentityProvider, customerId, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to get specific AuthConfig ID: {Id}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<AuthConfigAzureViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Specific AuthConfig retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<AuthConfigAzureViewModel>(response.Content);
                return IntegrationResult<AuthConfigAzureViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting specific AuthConfig ID: {Id} for Customer ID: {CustomerId}", id, customerId);
                return IntegrationResult<AuthConfigAzureViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> AuthenticateAzureAPI(LoginAzureAPI model)
        {
            _logger.LogInformation("Executing request to authenticate via Azure API. Configuration: {ConfigName}", model.ConfigAzureName);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.authenticateAzureAPI}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Azure API authentication failed. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Azure API authentication failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating via Azure API.");
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> ValidateAzureCredentials(Azurevalidate model)
        {
            _logger.LogInformation("Requesting Azure credentials validation for ClientId: {ClientId}", model.ClientId);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.validateAzureCredentials}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Azure credentials validation failed. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Azure credentials validation failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Azure credentials.");
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<string>> ValidateAzureToken(TokenRequestAzure model)
        {
            _logger.LogInformation("Requesting verification of Azure AD token.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.validateAzureToken}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Azure token validation request failed. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<string>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Azure token validation failed");
                }

                return IntegrationResult<string>.Success(response.Content ?? string.Empty, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing Azure token validation.");
                return IntegrationResult<string>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<bool>> DeleteAuthConfig(int id)
        {
            _logger.LogInformation("Sending request to delete AuthConfig ID: {Id}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.deleteIdentityProvider, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.DeleteAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to delete AuthConfig ID: {Id}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<bool>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "AuthConfig deletion failed");
                }

                return IntegrationResult<bool>.Success(true, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting AuthConfig ID: {Id}", id);
                return IntegrationResult<bool>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<AzureGroupRoleMappingViewModel>> InterpretProfile(AzureGroupRoleMappingViewModel model)
        {
            _logger.LogInformation("Sending request to interpret Azure profile mapping.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.interpretProfile}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to interpret Azure profile. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<AzureGroupRoleMappingViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profile interpretation failed");
                }

                var data = JsonConvert.DeserializeObject<AzureGroupRoleMappingViewModel>(response.Content);
                return IntegrationResult<AzureGroupRoleMappingViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interpreting Azure profile via integrated login service.");
                return IntegrationResult<AzureGroupRoleMappingViewModel>.Failure(500, ex.Message);
            }
        }
        #endregion

        #region PolicySystemProfile Methods
        public async Task<IntegrationResult<PolicySystemProfileViewModel>> GetProfileById(Guid id)
        {
            _logger.LogInformation("Requesting profile details for ID: {ProfileId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.getProfileById, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to retrieve profile {ProfileId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<PolicySystemProfileViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profile retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<PolicySystemProfileViewModel>(response.Content);
                return IntegrationResult<PolicySystemProfileViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile {ProfileId} from integrated login service.", id);
                return IntegrationResult<PolicySystemProfileViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<List<PolicySystemProfileViewModel>>> GetAllProfilesWithPage(int page = 0, int offset = 0, string filter = null)
        {
            _logger.LogInformation("Retrieving profiles list. Page: {Page}, Offset: {Offset}, Filter: {Filter}", page, offset, filter);
            try
            {
                string baseSiteUrl = string.IsNullOrEmpty(filter)
                    ? $"{BaseURL}{string.Format(PolicySystemUris.getAllProfilesWithPage, page, offset)}"
                    : $"{BaseURL}{string.Format(PolicySystemUris.getAllProfilesWithPageAndFilter, page, offset, Uri.EscapeDataString(filter))}";

                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to retrieve profiles list. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<List<PolicySystemProfileViewModel>>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profiles list retrieval failed");
                }

                var data = JsonConvert.DeserializeObject<List<PolicySystemProfileViewModel>>(response.Content);
                return IntegrationResult<List<PolicySystemProfileViewModel>>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profiles list from integrated login service.");
                return IntegrationResult<List<PolicySystemProfileViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<List<PolicySystemProfileViewModel>>> GetAllProfilesList()
        {
            _logger.LogInformation("Retrieving all profiles list without pagination.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.getAllProfilesList}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                request.Method = Method.Get;
                var response = await client.ExecuteAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to retrieve profiles. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<List<PolicySystemProfileViewModel>>.Failure((int)response.StatusCode, response.ErrorMessage ?? "Profiles retrieval failed");
                }

                // Configuração local de segurança para o Newtonsoft no projeto de Certificados
                var settings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };

                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PolicySystemProfileViewModel>>(response.Content, settings)
                           ?? new List<PolicySystemProfileViewModel>();

                return IntegrationResult<List<PolicySystemProfileViewModel>>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving full profiles list from integrated login service.");
                return IntegrationResult<List<PolicySystemProfileViewModel>>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemProfileViewModel>> CreateProfile(PolicySystemProfileViewModel model)
        {
            _logger.LogInformation("Sending request to create a new profile.");
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.addProfile}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to create profile. Status Code: {StatusCode}", response.StatusCode);
                    return IntegrationResult<PolicySystemProfileViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profile creation failed");
                }

                var data = JsonConvert.DeserializeObject<PolicySystemProfileViewModel>(response.Content);
                return IntegrationResult<PolicySystemProfileViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile via integrated login service.");
                return IntegrationResult<PolicySystemProfileViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<PolicySystemProfileViewModel>> EditProfile(PolicySystemProfileViewModel model)
        {
            _logger.LogInformation("Sending request to update profile ID: {ProfileId}", model?.IdProfile);
            try
            {
                string baseSiteUrl = $"{BaseURL}{PolicySystemUris.editProfile}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl).AddJsonBody(model);

                var response = await client.PutAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning("Failed to update profile ID: {ProfileId}. Status Code: {StatusCode}", model?.IdProfile, response.StatusCode);
                    return IntegrationResult<PolicySystemProfileViewModel>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profile update failed");
                }

                var data = JsonConvert.DeserializeObject<PolicySystemProfileViewModel>(response.Content);
                return IntegrationResult<PolicySystemProfileViewModel>.Success(data, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile ID: {ProfileId} via integrated login service.", model?.IdProfile);
                return IntegrationResult<PolicySystemProfileViewModel>.Failure(500, ex.Message);
            }
        }

        public async Task<IntegrationResult<bool>> DeleteProfile(Guid id)
        {
            _logger.LogInformation("Sending request to delete profile ID: {ProfileId}", id);
            try
            {
                string baseSiteUrl = $"{BaseURL}{string.Format(PolicySystemUris.deleteProfile, id)}";
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(baseSiteUrl);

                var response = await client.DeleteAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to delete profile ID: {ProfileId}. Status Code: {StatusCode}", id, response.StatusCode);
                    return IntegrationResult<bool>.Failure((int)response.StatusCode, response.Content ?? response.ErrorMessage ?? "Profile deletion failed");
                }

                return IntegrationResult<bool>.Success(true, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile ID: {ProfileId} via integrated login service.", id);
                return IntegrationResult<bool>.Failure(500, ex.Message);
            }
        }
        #endregion
    }
}