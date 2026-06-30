using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using EcosCLM.Domain.DataTypes;
using EcosCLM.Data.Services;
using EcosCLM.Application.Interfaces;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Application.Extensions;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using System.Threading.Tasks;
using EcosCLM.Web.EcosLoginIntegration.Extensions;

namespace EcosCLM.Web.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly IConfiguration _config;
        private readonly IEcosLoginService _EcosLoginService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly EmailService _emailService;
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ISyslogService _syslogService;

        private const string CustomerCookieName = "EcosPreference";

        public bool LoginUserAndPassword { get; set; } = true;
        public bool LoginAzureSSO { get; set; } = true;
        public bool LoginAzureIdentity { get; set; } = false;
        public bool Login2fa { get; set; } = false;
        public bool SessionExpired { get; set; }

        [BindProperty]
        public LoginViewModel ModelLogin { get; set; }

        public string ReturnUrl { get; set; }
        public Guid Customer { get; set; } = Guid.Empty;
        public string CustomerName { get; set; } = string.Empty;

        [BindProperty, BindNever]
        public PolicySystemUser UserLogin { get; set; }

        public LoginModel(
            IAuditLogsRepository auditLogs,
            ISyslogService syslogService,
            EmailService emailService,
            INotificationsRepository notificationsRepository,
            ILogger<LoginModel> logger,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IEcosLoginService EcosLoginService)
        {
            _auditLogs = auditLogs;
            _syslogService = syslogService;
            _emailService = emailService;
            _notificationsRepository = notificationsRepository;
            _logger = logger;
            _config = config;
            _EcosLoginService = EcosLoginService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null, bool expired = false)
        {
            _logger.LogInformation("LoginModel OnGet: {Time}", DateTime.UtcNow.ToLongTimeString());

            HandleSessionState(expired);
            ClearSessionAndCookies();

            ReturnUrl = returnUrl ?? Url.Content("~/");

            try
            {
                RestoreCustomerCookieFromTempData();
                await ResolveCustomerAsync(ReturnUrl);

                if (User.Identity?.IsAuthenticated == true && User.HasClaim(x => x.Type == ClaimTypes.NameIdentifier))
                {
                    return RedirectToPage("/Index");
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                _logger.LogError(ex, "API de Login está inacessível no OnGet.");
                ModelState.AddModelError(string.Empty, "O serviço de autenticação está temporariamente indisponível. Por favor, tente mais tarde.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnGet Exception: {Message}", ex.Message);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = "")
        {
            ReturnUrl = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/");

            ModelState.Remove("ModelLogin.ReturnUrl");
            ModelState.Remove("ModelLogin.VerificationCode");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Enter username and password!");
                return Page();
            }

            var loginRequest = new LoginViewModel
            {
                TxUsername = ModelLogin.TxUsername,
                TxPassword = ModelLogin.TxPassword,
                ReturnUrl = _config["AppSettings:Clients:Dashboard"] ?? HttpContext.Request.Host.Value
            };

            var tokenServiceResponse = await _EcosLoginService.Login(loginRequest);
            var userToken = JsonConvert.DeserializeObject<UserToken>(tokenServiceResponse.Data);

            if (string.IsNullOrEmpty(userToken?.token))
            {
                ModelState.AddModelError(string.Empty, ParseErrorMessage(tokenServiceResponse.Data));
                return Page();
            }

            return await ProcessAuthenticationAsync(userToken.token);
        }

        private async Task<IActionResult> ProcessAuthenticationAsync(string token)
        {
            var membershipContext = await _EcosLoginService.ValidateApiUserByToken(token);
            var user = membershipContext.Data?.User;

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid user context.");
                return Page();
            }

            await LoadAuthFlowConfigAsync(user.IdCustomer);

            if (user.IsAuth2fa == true || Login2fa)
            {
                return SetupAndRedirectTo2fa(user);
            }

            var claimsPrincipal = await BuildClaimsPrincipal(membershipContext.Data);
            var authProperties = BuildAuthenticationProperties();

            SetCustomerCookieIfRequired(user);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            AuditSuccessfulLogin(user.UserEmail);

            return LocalRedirect(ReturnUrl);
        }

        private async Task LoadAuthFlowConfigAsync(Guid customerId)
        {
            var baseUrl = _config["AppSettings:Clients:Login"];
            var uri = string.Format(PolicySystemUris.getClientAuthFlowConfig, customerId);
            var response = await HttpRequestService.GetAsync(string.Concat(baseUrl, uri), _logger);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<AuthFlowConfigViewModel>(responseContent);
                if (result != null)
                {
                    LoginUserAndPassword = result.DefaultAutenthicationEnabled;
                    LoginAzureSSO = result.MicrosoftAutenthicationEnabled;
                    LoginAzureIdentity = result.MicrosoftIdentityProviderEnabled;
                    Login2fa = result.MultFactorAuthenticationEnabled;
                }
            }
            else
            {
                TempData["warning"] = $"Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}.\n{responseContent}";
            }
        }

        private async Task ResolveCustomerAsync(string currentReturnUrl)
        {
            var customerIdentifier = ExtractCustomerFromUrl(currentReturnUrl);

            if (!string.IsNullOrEmpty(customerIdentifier))
            {
                Customer = await GetCustomerId(customerIdentifier);
                _logger.LogInformation("Customer identifier found in URL: '{Identifier}'", customerIdentifier);
            }
            else if (HttpContext.Request.Cookies.TryGetValue(CustomerCookieName, out var customerFromCookie) && Guid.TryParse(customerFromCookie, out var parsedCustomer))
            {
                Customer = parsedCustomer;
                _logger.LogInformation("Customer identifier found in cookie.");
            }

            if (Customer != Guid.Empty)
            {
                await LoadAuthFlowConfigAsync(Customer);
                var customerData = await _EcosLoginService.GetCustomerById(Customer);
                CustomerName = customerData.Data?.TxTitle ?? string.Empty;
            }
        }

        private void RestoreCustomerCookieFromTempData()
        {
            if (TempData.TryGetValue("CustomerPreferenceAfterLogout", out var tempValue) && tempValue != null)
            {
                var customerFromTempData = tempValue.ToString();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    SameSite = SameSiteMode.None
                };

                HttpContext.Response.Cookies.Append(CustomerCookieName, customerFromTempData, cookieOptions);
                _logger.LogInformation("Cookie '{CookieName}' recreated after logout.", CustomerCookieName);
            }
        }

        private IActionResult SetupAndRedirectTo2fa(ContextUserViewModel user)
        {
            TempData["FirstAcess"] = string.IsNullOrEmpty(user.Secret) ? "true" : "false";
            TempData["User"] = ModelLogin.TxUsername;
            TempData["Password"] = ModelLogin.TxPassword;
            TempData["GlobalSettings2fa"] = Login2fa ? "true" : "false";

            return RedirectToPage("Login2Fa", new { returnUrl = ReturnUrl });
        }

        private async Task<ClaimsPrincipal> BuildClaimsPrincipal(MembershipContext membershipContext)
        {
            var user = membershipContext.User;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Country, "enEN"),
                new Claim("Profile", user.Profile.ToString(), ClaimValueTypes.Integer)
            };

            if (membershipContext.Roles.AnyOrNull() && user.Profile == 0)
            {
                var encryptKey = _config.GetEncryptKeyFromConfig();
                var roles = await _EcosLoginService.EncryptRoles(membershipContext.Roles, encryptKey);
                claims.Add(new Claim(ClaimTypes.Role, roles.Data));
            }

            var customerConfig = await _EcosLoginService.GetCustomerById(user.IdCustomer);
            if (customerConfig.IsSuccessful)
            {
                claims.Add(new Claim("Customer", customerConfig.Data?.TxTitle));
            }
            else
            {
                _logger.LogWarning("Customer name not found for IdCustomer: {IdCustomer}.", user.IdCustomer);
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        private AuthenticationProperties BuildAuthenticationProperties()
        {
            if (!ModelLogin.LembrarMe) return null;

            return new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };
        }

        private async Task SetCustomerCookieIfRequired(ContextUserViewModel user)
        {
            var hasCustomerConfig = await _EcosLoginService.GetAuthConfigByCustomerId(user.IdCustomer);
            if (!hasCustomerConfig.Data.Any()) return;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.None
            };

            HttpContext.Response.Cookies.Append(CustomerCookieName, user.IdCustomer.ToString(), cookieOptions);
        }

        private async Task AuditSuccessfulLogin(string userEmail)
        {
            var user = await _EcosLoginService.ExistPolicySystemUserByEmail(userEmail);

            var auditLog = new AuditLogs
            {
                Date = DateTime.Now,
                User = userEmail,
                IdCustumer = user.Data?.IdCustomer ?? Guid.Empty,
                Log = $"User: {userEmail} logged in",
                LogType = "Authentication"
            };

            await _auditLogs.CreateAsync(auditLog);

            await _notificationsRepository.CreateAsync(new Notifications
            {
                Timestamp = DateTime.Now,
                User = userEmail,
                Message = "New Login",
                Link = "#",
                Icon = "security"
            });

            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                _emailService.ApiSendAuditEmailAsync(userEmail, ip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send audit email for user {UserEmail}", userEmail);
            }
        }

        private string ExtractCustomerFromUrl(string url)
        {
            try
            {
                var uri = new Uri(new Uri($"{Request.Scheme}://{Request.Host}"), url);
                var query = QueryHelpers.ParseQuery(uri.Query);

                if (query.TryGetValue("customer", out var customer))
                {
                    return customer.ToString();
                }
            }
            catch (UriFormatException ex)
            {
                _logger.LogWarning("Failed to parse URI from returnUrl '{Url}': {Message}", url, ex.Message);
            }
            return null;
        }

        private async Task<Guid> GetCustomerId(string customerName)
        {
            var customer = await _EcosLoginService.GetCustomerByName(customerName);
            return customer.Data?.IdCustomer ?? Guid.Empty;
        }

        private void HandleSessionState(bool expired)
        {
            SessionExpired = expired;
            if (expired)
            {
                TempData["ErrorMessage"] = "Your session has expired due to inactivity. Please login again.";
            }

            if (TempData.TryGetValue("ErrorMessage", out var message))
            {
                ModelState.AddModelError(string.Empty, message.ToString());
            }
        }

        private void ClearSessionAndCookies()
        {
            var context = _httpContextAccessor.HttpContext;
            context?.Session.Clear();

            const string appCookieName = "EcosCloudVhsmDashboard";
            if (context?.Request.Cookies.ContainsKey(appCookieName) == true)
            {
                context.Response.Cookies.Delete(appCookieName);
            }
        }

        private string ParseErrorMessage(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                if (doc.RootElement.TryGetProperty("message", out var messageElement))
                {
                    return messageElement.GetString() ?? "Erro desconhecido retornado pela API.";
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Captura especificamente erros de JSON malformado.
                // O nível é Warning porque não derruba o sistema, mas indica um comportamento inesperado da API.
                _logger.LogWarning(ex, "Falha ao realizar o parse do JSON de erro. Resposta bruta: {JsonResponse}", jsonResponse);
            }
            catch (Exception ex)
            {
                // Captura outras exceções imprevistas (ex: NullReference, OutOfMemory, etc)
                _logger.LogError(ex, "Erro inesperado ao tentar extrair a mensagem de erro do login.");
            }

            // Retorno padrão de segurança
            return "An unexpected error occurred during login. - Error Code: 0013";
        }
    }
}