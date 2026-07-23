namespace EcosCLM.Web.Infrastructure.Middlewares
{
    public class ForceHttpsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ForceHttpsMiddleware> _logger;
        private readonly bool _forceHttps;

        public ForceHttpsMiddleware(RequestDelegate next, ILogger<ForceHttpsMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _forceHttps = configuration.GetValue<bool>("AzureUseSSL");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_forceHttps && !context.Request.IsHttps)
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var proto) && proto.ToString().Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    context.Request.Scheme = "https";
                }
                else
                {
                    context.Request.Scheme = "https";
                    _logger.LogWarning("HTTPS forced natively. No valid X-Forwarded-Proto found.");
                }
            }

            await _next(context).ConfigureAwait(false);
        }
    }
}