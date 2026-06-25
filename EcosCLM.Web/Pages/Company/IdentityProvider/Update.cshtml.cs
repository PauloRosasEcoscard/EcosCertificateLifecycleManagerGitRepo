using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Data.Services;
using EcosCLM.Domain.DataTypes;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.EcosLoginIntegration.Services;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace EcosCLM.Web.Pages.Company.IdentityProvider
{
    public class UpdateModel : BasePageModel<AuthConfigAzureViewModel>
    {
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ISyslogService _syslogService;
        private readonly IEcosLoginService _ecosLoginService;
        ILogger<UpdateModel> _logger;
        IConfiguration _configuration;
        public SelectList Profiles { get; set; }
        private Dictionary<string, string> ProfilesDict;

        // Esta é a lista privada que recebe os dados do banco de dados em GetItem
        List<AzureGroupRoleMappingViewModel> azureRoleMappings { get; set; } = null;

        [BindProperty]
        public string AzureRoleMappingsJson { get; set; } // Esta é a propriedade pública que o Razor Page usa para o JSON

        public Dictionary<Guid, string> Customers;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration config,
            IAuditLogsRepository auditLogs,
            IEcosLoginService ecosLoginService,
            IHttpContextAccessor httpContextAccessor,
            ISyslogService syslogService)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _auditLogs = auditLogs;
            _configuration = config;
            _syslogService = syslogService;
            _ecosLoginService = ecosLoginService;

            _syslogService = syslogService;

        }

        public async Task<IActionResult> OnGet(int id)
        {
            await LoadCustomersAsync();
            await GetData(id);
            await GetListProfilesAsync();

            if (Item == null)
            {
                TempData["warning"] = "Item not found";
                return RedirectToPage("Index");
            }

            if (Item.IdCustomer != CustumerId)
            {
                TempData["warning"] = "Invalid operation";
                return RedirectToPage("Index");
            }

            if (azureRoleMappings != null)
            {
                AzureRoleMappingsJson = System.Text.Json.JsonSerializer.Serialize(azureRoleMappings, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase // Ensure camelCase for JavaScript
                });
            }
            else
            {
                AzureRoleMappingsJson = "[]"; // Ensure it's an empty JSON array if no mappings exist
            }

            _logger.LogInformation($"[OnGet] Final AzureRoleMappingsJson content prepared: {AzureRoleMappingsJson}");

            return Page();
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var result = await _ecosLoginService.GetAllCustomers();
                if (result.IsSuccessful && result.Data != null)
                {
                    Customers = result.Data.ToDictionary(x => x.IdCustomer, x => x.TxTitle);
                }
                else
                {
                    _logger.LogWarning("Failed to load customers list. Status: {StatusCode}", result.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building customers dictionary.");
            }
        }

        private async Task GetListProfilesAsync()
        {
            // Inicializa o dicionário com os perfis fixos/internos
            ProfilesDict.Clear();
            ProfilesDict.Add("1", "Admin");
            ProfilesDict.Add("2", "Audit");

            var profileList = await _ecosLoginService.GetAllProfilesList();

            if (profileList != null)
            {
                var dbProfiles = profileList.Data
                    .Where(x => x.TxTitle != "Admin")
                    .ToDictionary(x => x.IdProfile, x => x.TxTitle);

                foreach (var dbProfile in dbProfiles)
                {
                    ProfilesDict.Add(dbProfile.Key.ToString(), dbProfile.Value);
                }
            }

            Profiles = new SelectList(ProfilesDict, "Key", "Value");

        }

        public async Task<IActionResult> OnPostSave()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await UpdateAzureAuthConfig();
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex.Message);
                }
            }

            if (string.IsNullOrEmpty(AzureRoleMappingsJson) && azureRoleMappings != null)
            {
                AzureRoleMappingsJson = System.Text.Json.JsonSerializer.Serialize(azureRoleMappings, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });
            }
            else if (string.IsNullOrEmpty(AzureRoleMappingsJson)) // Ensure it's not null if there are no mappings
            {
                AzureRoleMappingsJson = "[]";
            }

            _logger.LogInformation($"[OnPostSave] AzureRoleMappingsJson content after failed post: {AzureRoleMappingsJson}");
            return Page();
        }

        public async Task<IActionResult> OnPostTest()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await TestAzureAuthConfig();
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex.Message);
                }
            }
            // *******************************************************************
            // CRITICAL FIX: Re-populate AzureRoleMappingsJson for test post
            // *******************************************************************
            if (string.IsNullOrEmpty(AzureRoleMappingsJson) && azureRoleMappings != null)
            {
                AzureRoleMappingsJson = System.Text.Json.JsonSerializer.Serialize(azureRoleMappings, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });
            }
            else if (string.IsNullOrEmpty(AzureRoleMappingsJson))
            {
                AzureRoleMappingsJson = "[]";
            }
            _logger.LogInformation($"[OnPostTest] AzureRoleMappingsJson content after test post: {AzureRoleMappingsJson}");
            return Page();
        }

        private async Task TestAzureAuthConfig()
        {
            string url = _configuration.GetSection("AppSettings:Clients:Login").Value;
            string uri = PolicySystemUris.validateAzureCredentials;

            var body = new
            {
                DiscoveryUri = Item.DiscoveryUri,
                ClientId = Item.ClientId,
                ClientSecret = Item.ClientSecret
            };

            HttpResponseMessage response = await HttpRequestService.PostAsync(string.Concat(url, uri), body, _logger);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Identity provider validated!";
            }
            else
            {
                TempData["warning"] = "Unable to validate the identity provider, check if every information is correct.";
            }
        }

        async Task GetData(int id)
        {
            try
            {
                await GetItem(id);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex.Message);
            }
        }

        private async Task GetItem(int id)
        {
            string url = _configuration.GetSection("AppSettings:Clients:Login").Value;
            string uri = string.Format(PolicySystemUris.getIdentityProviderByCustumerIdentityProvider, CustumerId, id);
            HttpResponseMessage response = await HttpRequestService.GetAsync(string.Concat(url, uri), _logger);

            string responseContentHttp = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Item = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthConfigAzureViewModel>(responseContentHttp);
            }
            else
            {
                TempData["warning"] = $"Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}.\n{responseContentHttp}|";
            }

            // *******************************************************************
            // CRITICAL FIX: Ensure Item?.Id is included in the URI for Azure Group Role Mapping
            // This is crucial for fetching mappings specific to the current Identity Provider.
            // *******************************************************************
            string uriAzureGroupRole = string.Format(PolicySystemUris.getAzureGroupRoleMapping, CustumerId, Item?.Id); // Added Item?.Id

            _logger.LogInformation($"[GetItem] Fetching Azure Group Role Mappings from: {string.Concat(url, uriAzureGroupRole)}");
            HttpResponseMessage responseAzureGroupRole = await HttpRequestService.GetAsync(string.Concat(url, uriAzureGroupRole), _logger);
            string responseContentHttpAzureGroupRole = await responseAzureGroupRole.Content.ReadAsStringAsync();

            if (responseAzureGroupRole.IsSuccessStatusCode)
            {
                // This line populates the private 'azureRoleMappings' list
                azureRoleMappings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AzureGroupRoleMappingViewModel>>(responseContentHttpAzureGroupRole);
                _logger.LogInformation($"[GetItem] Azure Group Role Mappings loaded successfully. Count: {azureRoleMappings?.Count ?? 0}");
            }
            else
            {
                TempData["warning"] = $"Failed to load Azure group role mappings: Status Code: {(int)responseAzureGroupRole.StatusCode} - {responseAzureGroupRole.ReasonPhrase}.";
                _logger.LogError($"[GetItem] Failed to load Azure group role mappings for customer {CustumerId} and ID {Item?.Id}. Response: {responseContentHttpAzureGroupRole}");
            }
        }

        private async Task UpdateAzureAuthConfig()
        {
            Item.IdCustomer = CustumerId;

            var configResult = await _ecosLoginService.EditAuthConfig(Item);

            if (configResult.IsSuccessful && configResult.Data != null)
            {
                var createdOrUpdatedAuth = configResult.Data;

                if (createdOrUpdatedAuth.Id == 0)
                {
                    TempData["error"] = "Não foi possível obter o ID do Identity Provider para salvar os mapeamentos.";
                    return;
                }

                List<AzureGroupRoleMappingViewModel> azureRoleMappingsToSave = null;
                if (!string.IsNullOrEmpty(AzureRoleMappingsJson))
                {
                    try
                    {
                        azureRoleMappingsToSave = System.Text.Json.JsonSerializer.Deserialize<List<AzureGroupRoleMappingViewModel>>(AzureRoleMappingsJson, new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Erro ao desserializar AzureRoleMappingsJson do frontend.");
                        TempData["warning"] = "Erro ao processar mapeamentos de Azure. Por favor, tente novamente.";
                        return;
                    }
                }

                if (azureRoleMappingsToSave == null)
                {
                    azureRoleMappingsToSave = new List<AzureGroupRoleMappingViewModel>();
                }

                var interpretationTasks = azureRoleMappingsToSave.Select(async mapping =>
                {
                    mapping.AuthConfigAzureId = createdOrUpdatedAuth.Id;

                    var result = await _ecosLoginService.InterpretProfile(mapping);
                    if (result.IsSuccessful && result.Data != null)
                    {
                        mapping.InternalProfileType = result.Data.InternalProfileType;
                        mapping.PolicySystemProfileId = result.Data.PolicySystemProfileId;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to interpret mapping for ExternalId: {ExternalId}. Status: {StatusCode}", mapping.ExternalId, result.StatusCode);
                    }
                });

                await Task.WhenAll(interpretationTasks);

                var mappingResult = await _ecosLoginService.UpdateAzureGroupRoleMappings(azureRoleMappingsToSave);

                if (!mappingResult.IsSuccessful)
                {
                    TempData["warning"] = $"Identity provider updated, but failed to update role mappings. Status Code: {mappingResult.StatusCode}. Details: {mappingResult.ErrorMessage}";
                }
                else
                {
                    TempData["success"] = "Identity provider and role mappings updated successfully!";
                }

                _auditLogs.Create(new AuditLogs
                {
                    Date = DateTime.Now,
                    User = Email,
                    IdCustumer = CustumerId,
                    Log = $"User: {Email} edited the Identity Provider Configuration",
                    LogType = "Company Management"
                }, _syslogService, HttpContextAccessor);
            }
            else
            {
                TempData["warning"] = $"Status Code: {configResult.StatusCode} - {configResult.ErrorMessage}|";
            }
        }
    }
}
