using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Data.Services;
using EcosCLM.Domain.DataTypes;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Company.Users
{
    public class AddModel : BasePageModel<PolicySystemUserViewModel>
    {
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ILogger<AddModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEcosLoginService _EcosLoginService;
        private readonly ISyslogService _syslogService;

        private Dictionary<string, string> ProfilesDict;
        public SelectList Profiles { get; set; }

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IConfiguration config,
            IAuditLogsRepository auditLogs,
            IHttpContextAccessor httpContextAccessor, 
            IEcosLoginService EcosLoginService,
            ISyslogService syslogService)
            : base(EcosLoginService, config)
        {
            _logger = logger;
            _auditLogs = auditLogs;
            _configuration = configuration;
            _syslogService = syslogService;
            _EcosLoginService = EcosLoginService;
            
        }


        public async Task<IActionResult> OnGet()
        {
            await GetListProfilesAsync();
            return Page();
        }

        private async Task GetListProfilesAsync()
        {
            ProfilesDict ??= new Dictionary<string, string>();
            ProfilesDict.Clear();
            ProfilesDict.Add("1", "Admin");
            ProfilesDict.Add("2", "Audit");

            var profileList = await _EcosLoginService.GetAllProfilesList();

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

        public async Task<IActionResult> OnPost()
        {
            await GetListProfilesAsync();
            var teste1 = ModelState.TryGetValue("Item.TxPassword", out var entryA);
            var teste2 = ModelState.TryGetValue("Item.TxEmail", out var entryB);
            var login = Item.TxEmail;
            var senha = Item.TxPassword;
            if (ModelState.TryGetValue("Item.TxPassword", out var entry) && entry.ValidationState == ModelValidationState.Valid
                && ModelState.TryGetValue("Item.TxEmail", out var entry1) && entry1.ValidationState == ModelValidationState.Valid)
            {
                try
                {
                    if (Item.IdProfile == "1" || Item.IdProfile == "2")
                    {
                        Item.Profile = int.Parse(Item.IdProfile);
                    }
                    else
                    {
                        Item.Profile = 0;
                    }

                    await CreateUser();
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex.Message);
                }

                return RedirectToPage("Index");
            }
            return Page();
        }

        private async Task CreateUser()
        {
            Item.Secret = "";
           var response = await _EcosLoginService.AddPolicySystemUser(Item);

            if (response.IsSuccessful)
            {
                _auditLogs.Create(new AuditLogs
                {
                    Date = DateTime.Now,
                    User = Email,
                    IdCustumer = CustumerId,
                    Log = "User: " + Email + " created a new user",
                    LogType = "Company Management"
                }, _syslogService, HttpContextAccessor);
            }
            else
            {
                if (!string.IsNullOrEmpty(response.ErrorMessage) && response.ErrorMessage.Contains("errors"))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(response.ErrorMessage);
                        if (doc.RootElement.TryGetProperty("errors", out var errorsElement) &&
                            errorsElement.TryGetProperty("TxEmail", out var emailErrors))
                        {
                            TempData["warning"] = emailErrors[0].GetString();
                            return;
                        }
                    }
                    catch { }
                }

                TempData["warning"] = "Email already registered or invalid data provided.";
            }
        }
    }
}
