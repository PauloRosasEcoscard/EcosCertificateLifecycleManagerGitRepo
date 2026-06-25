using EcosCLM.Web.Infrastructure.Extensions;

namespace EcosCLM.Web.Infrastructure.Middlewares
{
    public class CustomerIdentificationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomerIdentificationMiddleware> _logger;

        public CustomerIdentificationMiddleware(RequestDelegate next, ILogger<CustomerIdentificationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verifica se o HttpContext já tem o cliente. Se não, busca o valor.
            if (!context.Items.ContainsKey("CustomerName"))
            {
                var customerToUse = context.GetCustomerFromRequest(_logger);

                if (!string.IsNullOrWhiteSpace(customerToUse))
                {
                    // Armazena o cliente no HttpContext.Items para uso posterior
                    context.Items["CustomerName"] = customerToUse;
                    _logger.LogInformation("Middleware: Customer '{CustomerName}' identified and stored in HttpContext.Items.", customerToUse);
                }
                else
                {
                    _logger.LogInformation("Middleware: No specific customer found in URL or cookie.");
                }
            }

            await _next(context);
        }
    }
}
