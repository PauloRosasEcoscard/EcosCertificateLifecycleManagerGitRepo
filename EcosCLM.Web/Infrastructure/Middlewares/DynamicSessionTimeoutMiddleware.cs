using EcosCLM.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EcosCLM.Web.Infrastructure.Middlewares;

public class DynamicSessionTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DynamicSessionTimeoutMiddleware> _logger;

    public DynamicSessionTimeoutMiddleware(RequestDelegate next, ILogger<DynamicSessionTimeoutMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IConfigurationService configurationService)
    {
        if (context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/lib"))
        {
            await _next(context);
            return;
        }

        if (!context.Features.Get<Microsoft.AspNetCore.Http.Features.ISessionFeature>()?.Session.IsAvailable ?? true)
        {
            await _next(context);
            return;
        }

        string? customerName = context.Items["CustomerName"] as string;

        if (string.IsNullOrEmpty(customerName))
        {
            await _next(context);
            return;
        }

        var timeoutMinutes = await configurationService.GetSessionTimeoutMinutesAsync(customerName);
        var timeout = TimeSpan.FromMinutes(timeoutMinutes);
        var lastActivityString = context.Session.GetString("LastActivity");

        if (string.IsNullOrEmpty(lastActivityString))
        {
            context.Session.SetString("LastActivity", DateTimeOffset.UtcNow.ToString());
        }
        else
        {
            var lastActivity = DateTimeOffset.Parse(lastActivityString);

            if ((DateTimeOffset.UtcNow - lastActivity) > timeout)
            {
                context.Session.Clear();
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Response.Redirect("/Authentication/Login?expired=true");
                return;
            }

            context.Session.SetString("LastActivity", DateTimeOffset.UtcNow.ToString());
        }

        await _next(context);
    }
}