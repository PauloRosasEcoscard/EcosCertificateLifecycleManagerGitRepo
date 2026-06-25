namespace EcosCLM.Web.Infrastructure.Extensions
{

    public static class CustomerIdentificationExtensions
    {
        private const string CustomerCookieName = "EcosPreference";

        /// <summary>
        /// Tries to identify the customer from the URL query parameter or from a specific cookie.
        /// URL query parameter takes precedence over the cookie.
        /// </summary>
        /// <param name="httpContext">The current HttpContext.</param>
        /// <param name="logger">The logger instance for logging information.</param>
        /// <returns>The identified customer string, or null if not found.</returns>
        public static string GetCustomerFromRequest(this HttpContext httpContext, ILogger logger)
        {
            string customerFromUrl = null;
            string customerFromCookie = null;
            string customerToUse = null;

            if (httpContext == null)
            {
                logger.LogWarning("GetCustomerFromRequest: HttpContext is null. Cannot identify customer.");
                return null;
            }

            // 1. Try to get customer from URL query parameter
            if (httpContext.Request.Query.TryGetValue("customer", out var customerQueryValue))
            {
                customerFromUrl = customerQueryValue.ToString();
                logger.LogInformation("GetCustomerFromRequest: 'Customer' found in URL query: '{CustomerUrl}'", customerFromUrl);
            }

            // 2. Try to get customer from cookie
            if (httpContext.Request.Cookies.TryGetValue(CustomerCookieName, out var cookieValue))
            {
                customerFromCookie = cookieValue;
                logger.LogInformation("GetCustomerFromRequest: 'Customer' found in cookie: '{CustomerCookie}'", customerFromCookie);
            }

            // Determine which customer to use: URL has priority, then cookie
            if (!string.IsNullOrWhiteSpace(customerFromUrl))
            {
                customerToUse = customerFromUrl;
                logger.LogInformation("GetCustomerFromRequest: Using customer from URL: '{CustomerToUse}'", customerToUse);
            }
            else if (!string.IsNullOrWhiteSpace(customerFromCookie))
            {
                customerToUse = customerFromCookie;
                logger.LogInformation("GetCustomerFromRequest: Using customer from cookie: '{CustomerToUse}'", customerToUse);
            }
            else
            {
                logger.LogInformation("GetCustomerFromRequest: No specific customer provided in URL or cookie.");
            }

            return customerToUse;
        }
    }
}
