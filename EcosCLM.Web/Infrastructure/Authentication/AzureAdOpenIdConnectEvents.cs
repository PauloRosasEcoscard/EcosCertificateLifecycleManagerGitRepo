using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace EcosCLM.Web.Infrastructure.Authentication;

public class AzureAdOpenIdConnectEvents : OpenIdConnectEvents
{
    private readonly IConfiguration _configuration;
    private readonly IEcosLoginService _ecosLoginService;

    public AzureAdOpenIdConnectEvents(IEcosLoginService ecosLoginService, IConfiguration configuration)
    {
        _configuration = configuration;
        _ecosLoginService = ecosLoginService;
    }

    public override async Task RedirectToIdentityProvider(RedirectContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            await ClearAuthenticationState(context.HttpContext);
        }

        string customerToUse = context.HttpContext.Items["CustomerName"]?.ToString() ?? string.Empty;
        string clientIdToUse = _configuration["AzureAd:ClientId"]!;

        if (!string.IsNullOrWhiteSpace(customerToUse))
        {
            var customConfig = _ecosLoginService.GetAuthConfigByCustomerId(Guid.Parse(customerToUse)).Result.Data.FirstOrDefault();
            if (customConfig != null)
            {
                context.Options.Authority = customConfig.DiscoveryUri;
                context.Options.ClientSecret = customConfig.ClientSecret;
                context.ProtocolMessage.RedirectUri = $"https://{context.Request.Host}{_configuration["AzureAd:CallbackPath"]}";
                context.Options.ClientId = customConfig.ClientId;
                context.ProtocolMessage.ClientId = customConfig.ClientId;
                clientIdToUse = customConfig.ClientId;
            }
        }

        context.Properties.Items["ExpectedAudience"] = clientIdToUse;
        await Task.CompletedTask;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

        if (!context.Properties.Items.TryGetValue("ExpectedAudience", out var expectedAudience) || string.IsNullOrWhiteSpace(expectedAudience))
        {
            context.Fail("ExpectedAudience not found.");
            return;
        }

        if (context.SecurityToken is not System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwtToken)
        {
            context.Fail("Invalid token.");
            return;
        }

        if (!jwtToken.Audiences.Contains(expectedAudience))
        {
            context.Fail("InvalidAudience");
            return;
        }

        if (context.Properties.Items.TryGetValue("Customer", out var customerString) && customerString != null)
        {
            var identity = (ClaimsIdentity)context.Principal!.Identity!;
            if (!identity.HasClaim(c => c.Type == "CustomerName"))
            {
                identity.AddClaim(new Claim("CustomerName", customerString.ToString()!));
            }
        }

        await Task.CompletedTask;
    }

    public override async Task RemoteFailure(RemoteFailureContext context)
    {
        await ClearAuthenticationState(context.HttpContext);

        string errorMessage = context.Failure is SecurityTokenInvalidAudienceException
            ? "InvalidAudience"
            : "AuthenticationFailed";

        context.Response.Redirect($"/Authentication/Login?error={errorMessage}");
        context.HandleResponse();
    }

    private static async Task ClearAuthenticationState(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        context.User = new ClaimsPrincipal(new ClaimsIdentity());

        if (context.Features.Get<ISessionFeature>() != null)
        {
            context.Session.Clear();
        }
    }
}