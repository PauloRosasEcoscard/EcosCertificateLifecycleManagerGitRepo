using AutoMapper;
using EcosCLM.Application.Infrastructure.Mappers;
using EcosCLM.Data.Context;
using EcosCLM.Data.Extensions;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Services;
using EcosCLM.Web.Infrastructure.Exceptions;
using EcosCLM.Web.Infrastructure.Extensions;
using EcosCLM.Web.Infrastructure.Middlewares;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .ReadFrom.Configuration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host application bootstrapping...");
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<EcosCLMContext>("Database_Check")
                .AddAsyncCheck("EcosLogin_API_Check", async () =>
                {
                    try
                    {
                        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                        var loginApiUrl = builder.Configuration["AppSettings:Clients:Login"];
                        var response = await client.GetAsync(loginApiUrl);

                        return response.StatusCode != System.Net.HttpStatusCode.NotFound || response.IsSuccessStatusCode
                            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Login server active.")
                            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy($"Unexpected status: {response.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Connection failed.", ex);
                    }
                });

            builder.Services.AddDataProtection()
                .PersistKeysToDbContext<EcosCLMContext>()
                .SetApplicationName("EcosDashboard");

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddRazorPages().AddSessionStateTempDataProvider();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Services.AddHttpClient();

            var mapperInstance = MappingProfiles.LoadConfigurations();
            builder.Services.AddSingleton<IMapper>(mapperInstance);
            builder.Services.AddSingleton(mapperInstance);

            builder.Services.AddCustomAuthentication(builder.Configuration);
            builder.Services.AddCustomAuthorization();

            ServiceCollectionsExtension.ServicesApplication(builder.Services, builder.Configuration);
            builder.Services.AddScoped<IEcosLoginService, EcosLoginService>();
            builder.Services.AddSingleton<IEncryptionService, EncryptionService>();

            var app = builder.Build();

            app.UseForwardedHeaders();
            app.UseExceptionHandler();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            app.UseForceHttpsMiddleware();
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseCookiePolicy();

            app.UseCustomerIdentification();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<DynamicSessionTimeoutMiddleware>();

            app.MapRazorPages();

            app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        duration = report.TotalDuration,
                        info = report.Entries.Select(entry => new
                        {
                            key = entry.Key,
                            status = entry.Value.Status.ToString(),
                            error = entry.Value.Exception?.Message
                        })
                    };
                    await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented));
                }
            });

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "CRITICAL ERROR: Application failed to start.");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public class EcosDashboardContextFactory : IDesignTimeDbContextFactory<EcosCLMContext>
    {
        public EcosCLMContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var provider = config["RepositoryType"]?.ToUpper();
            var optionsBuilder = new DbContextOptionsBuilder<EcosCLMContext>();
            string connectionString = config.GetConnectionString("EcosCLM")!;

            if (provider == "MYSQL")
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), opt => opt.MigrationsAssembly("EcosCLM.Migrations.MYSQL"));
            else
                optionsBuilder.UseSqlServer(connectionString, opt => opt.MigrationsAssembly("EcosCLM.Migrations.SQL"));

            return new EcosCLMContext(optionsBuilder.Options, null, null);
        }
    }
}