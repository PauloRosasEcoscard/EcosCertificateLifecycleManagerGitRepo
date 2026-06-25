using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace EcosCLM.Web.Pages.Company.IdentityProvider
{
    public class AddModel : BasePageModel<AuthConfigAzureViewModel>
    {
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ISyslogService _syslogService;
        private readonly IEcosLoginService _ecosLoginService;
        private readonly ILogger<AddModel> _logger;

        public Dictionary<Guid, string> Customers { get; set; } = new();
        public Dictionary<string, string> ProfilesDict { get; set; } = new();
        public SelectList Profiles { get; set; }

        [BindProperty]
        public string AzureRoleMappingsJson { get; set; }

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IAuditLogsRepository auditLogs,
            ISyslogService syslogService)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _auditLogs = auditLogs;
            _syslogService = syslogService;
            _ecosLoginService = ecosLoginService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadInitialDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            ModelState.Remove("AzureRoleMappingsJson");
            if (ModelState.IsValid)
            {
                try
                {
                    if (Item.MappingProfileFor != 0 && string.IsNullOrEmpty(AzureRoleMappingsJson))
                    {
                        TempData["error"] = "enter all mapping information";
                        await LoadInitialDataAsync();
                        return Page();
                    }

                    await UpdateAuthConfig();
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error processing identity provider creation during post.");
                    await LoadInitialDataAsync();
                    return Page();
                }
            }

            await LoadInitialDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostTestAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await TestAuthConfig();
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error during identity provider validation test.");
                }
            }

            await LoadInitialDataAsync();
            return Page();
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                var customersResult = await _ecosLoginService.GetAllCustomers();
                if (customersResult.IsSuccessful && customersResult.Data != null)
                {
                    Customers = customersResult.Data.ToDictionary(x => x.IdCustomer, x => x.TxTitle);
                }

                ProfilesDict.Clear();
                ProfilesDict.Add("1", "Admin");
                ProfilesDict.Add("2", "Audit");

                var profilesResult = await _ecosLoginService.GetAllProfilesList();
                if (profilesResult.IsSuccessful && profilesResult.Data != null)
                {
                    var dbProfiles = profilesResult.Data
                        .Where(x => x.TxTitle != "Admin")
                        .ToDictionary(x => x.IdProfile, x => x.TxTitle);

                    foreach (var dbProfile in dbProfiles)
                    {
                        ProfilesDict.Add(dbProfile.Key.ToString(), dbProfile.Value);
                    }
                }

                Profiles = new SelectList(ProfilesDict, "Key", "Value");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading static and auxiliary data for Identity Provider.");
            }
        }

        private async Task UpdateAuthConfig()
        {
            Item.IdCustomer = CustumerId;

            var configResult = await _ecosLoginService.CreateAuthConfig(Item);

            if (configResult.IsSuccessful && configResult.Data != null)
            {
                var createdModels = configResult.Data;

                List<AzureGroupRoleMappingViewModel> azureRoleMappings = null;
                if (!string.IsNullOrEmpty(AzureRoleMappingsJson))
                {
                    try
                    {
                        azureRoleMappings = JsonSerializer.Deserialize<List<AzureGroupRoleMappingViewModel>>(AzureRoleMappingsJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Erro ao desserializar AzureRoleMappingsJson.");
                        TempData["warning"] = "Erro ao processar mapeamentos de Azure. Por favor, tente novamente.";
                        return;
                    }
                }

                if (azureRoleMappings != null && azureRoleMappings.Any())
                {
                    var interpretationTasks = azureRoleMappings.Select(async mapping =>
                    {
                        mapping.AuthConfigAzureId = createdModels.Id;

                        var interpretResult = await _ecosLoginService.InterpretProfile(mapping);
                        if (interpretResult.IsSuccessful && interpretResult.Data != null)
                        {
                            mapping.InternalProfileType = interpretResult.Data.InternalProfileType;
                            mapping.PolicySystemProfileId = interpretResult.Data.PolicySystemProfileId;
                        }
                        else
                        {
                            _logger.LogWarning("Failed to interpret mapping for ExternalId: {ExternalId}", mapping.ExternalId);
                        }
                    });

                    await Task.WhenAll(interpretationTasks);

                    var mappingResult = await _ecosLoginService.CreateAzureGroupRoleMappings(azureRoleMappings);
                    if (!mappingResult.IsSuccessful)
                    {
                        _logger.LogError("Failed to save role mappings for Identity Provider ID: {ProviderId}", createdModels.Id);
                    }
                }

                _auditLogs.Create(new AuditLogs
                {
                    Date = DateTime.Now,
                    User = Email,
                    IdCustumer = CustumerId,
                    Log = $"User: {Email} edited the Identity Provider Configuration",
                    LogType = "Company Management"
                }, _syslogService, HttpContextAccessor);

                TempData["success"] = "Identity provider updated successfully!";
            }
            else
            {
                TempData["warning"] = $"Status Code: {configResult.StatusCode} - {configResult.ErrorMessage}|";
            }
        }

        private async Task TestAuthConfig()
        {
            var validateModel = new Azurevalidate
            {
                DiscoveryUri = Item.DiscoveryUri,
                ClientId = Item.ClientId,
                ClientSecret = Item.ClientSecret
            };

            var response = await _ecosLoginService.ValidateAzureCredentials(validateModel);

            if (response.IsSuccessful)
            {
                TempData["success"] = "Identity provider validated!";
            }
            else
            {
                TempData["warning"] = "Unable to validate the identity provider, check if every information is correct.";
            }
        }
    }
}