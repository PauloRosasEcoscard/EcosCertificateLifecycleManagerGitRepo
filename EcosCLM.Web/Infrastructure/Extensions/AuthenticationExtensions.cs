using EcosCLM.Web.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace EcosCLM.Web.Infrastructure.Extensions;

public static class AuthenticationExtensions
{
    public static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
        {
            opt.ExpireTimeSpan = TimeSpan.FromHours(1);
            opt.Cookie.Name = "EcosDashboard";
            opt.LoginPath = "/Authentication/login";
            opt.LogoutPath = "/Authentication/logout";
            opt.AccessDeniedPath = "/Authentication/accessdenied/";
            opt.SlidingExpiration = true;
        });

        if (configuration.GetSection("AzureAd").Exists())
        {
            authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = $"{configuration["AzureAd:Instance"]}organizations/v2.0";
                options.ClientId = configuration["AzureAd:ClientId"];
                options.ClientSecret = configuration["AzureAd:ClientSecret"];
                options.CallbackPath = configuration["AzureAd:CallbackPath"];
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.UsePkce = true;
                options.RequireHttpsMetadata = false;

                options.Scope.Add("email");
                options.Scope.Add("openid");
                options.Scope.Add("profile");

                options.EventsType = typeof(AzureAdOpenIdConnectEvents); // Uses the extracted class

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    IssuerValidator = (issuer, securityToken, validationParameters) =>
                    {
                        if (issuer.StartsWith("https://login.microsoftonline.com/"))
                            return issuer;
                        throw new SecurityTokenInvalidIssuerException($"Issuer {issuer} is invalid.");
                    },
                    ValidateAudience = false,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

            services.AddScoped<AzureAdOpenIdConnectEvents>();
        }
    }

    public static void AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
    }
}