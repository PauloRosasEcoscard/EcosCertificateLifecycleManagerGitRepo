using EcosCLM.Web.Infrastructure.Middlewares;

namespace EcosCLM.Web.Infrastructure.Extensions
{
    public static class ForceHttpsMiddlewareExtensions
    {
        public static IApplicationBuilder UseForceHttpsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ForceHttpsMiddleware>();
        }
    }
}
