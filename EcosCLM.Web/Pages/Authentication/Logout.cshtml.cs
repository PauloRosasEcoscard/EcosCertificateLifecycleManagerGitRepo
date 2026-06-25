using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcosCLM.Web.Pages.Authentication
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LogoutModel> _logger;
        private const string CustomerCookieName = "EcosPreference";

        public LogoutModel(IConfiguration configuration,
                           IHttpContextAccessor httpContextAccessor,
                           ILogger<LogoutModel> logger)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("LogoutModel OnGetAsync started.");

            string customerPreferenceValue = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(CustomerCookieName, out var cookieValue))
            {
                customerPreferenceValue = cookieValue;
                _logger.LogInformation($"Customer cookie '{CustomerCookieName}' found with value: '{customerPreferenceValue}'.");
            }
            else
            {
                _logger.LogWarning($"Customer cookie '{CustomerCookieName}' not found on logout initiated.");
            }

            // Limpar sessões e outros cookies.
            _logger.LogInformation("Calling ClearOtherCookies().");
            ClearOtherCookies();
            _logger.LogInformation("ClearOtherCookies() completed.");

            // Realizar o logout do Azure AD e do esquema de autenticação principal.
            try
            {
                _logger.LogInformation("Calling SignOutAsync for Cookie and OpenIdConnect schemes.");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                _logger.LogInformation("SignOutAsync calls completed.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
            }

            if (!string.IsNullOrEmpty(customerPreferenceValue))
            {                
                TempData["CustomerPreferenceAfterLogout"] = customerPreferenceValue;
                _logger.LogInformation($"Customer preference '{customerPreferenceValue}' saved to TempData.");
            }

            string hsmSession = HttpContext.Session.GetString("HSMSession") ?? "";
            if (!string.IsNullOrEmpty(hsmSession))
            {
                HttpContext.Session.Remove("HSMSession");
                HttpContext.Session.Remove("VHSM");
                _logger.LogInformation("HSM session data removed.");
            }

            if (IsAuthenticatedAzure())
            {
                var postLogoutRedirectUri = _configuration["AzureAd:PostLogoutRedirectUri"];
                _logger.LogInformation($"User was authenticated via Azure AD. Redirecting to Azure logout endpoint: {postLogoutRedirectUri}");
                return Redirect($"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri)}");
            }

            _logger.LogInformation("Redirecting to /authentication/login (non-Azure logout path).");
            return Redirect("/authentication/login");
        }

        private bool IsAuthenticatedAzure()
        {
            var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            var hasAzureClaim = HttpContext.User.HasClaim(c => c.Type == "AzureAuth" && c.Value == "True");
            _logger.LogInformation($"IsAuthenticatedAzure check: IsAuthenticated={isAuthenticated}, HasAzureClaim={hasAzureClaim}");
            return isAuthenticated && hasAzureClaim;
        }

        private void ClearOtherCookies()
        {
            _logger.LogInformation("Clearing HTTP session.");
            _httpContextAccessor.HttpContext.Session.Clear(); // Limpa a sessão

            _logger.LogInformation("Attempting to delete all received cookies EXCEPT EcosCustomerPreference.");
            var cookies = _httpContextAccessor.HttpContext.Request.Cookies;
            foreach (var cookieName in cookies.Keys)
            {
                if (cookieName.Equals(CustomerCookieName, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"Skipping deletion of protected cookie: {cookieName}");
                    continue; // Pula este cookie
                }

                _logger.LogInformation($"Deleting cookie: {cookieName}");
                try
                {
                    // Deleta o cookie.
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(-1),
                        Path = "/"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting cookie '{cookieName}'.");
                }
            }
            _logger.LogInformation("Finished attempting to delete other cookies.");
        }
    }
}
