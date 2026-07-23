using EcosCLM.Web.Infrastructure.Middlewares;

namespace EcosCLM.Web.Infrastructure.Extensions
{
    public static class CustomerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomerIdentification(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomerIdentificationMiddleware>();
        }
    }
}
