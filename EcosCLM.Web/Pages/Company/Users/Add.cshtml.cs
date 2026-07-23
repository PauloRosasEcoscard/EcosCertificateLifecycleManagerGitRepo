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
            await GetListProfilesAsync().ConfigureAwait(false);
            return Page();
        }

        private async Task GetListProfilesAsync()
        {
            ProfilesDict ??= new Dictionary<string, string>();
            ProfilesDict.Clear();
            ProfilesDict.Add("1", "Admin");
            ProfilesDict.Add("2", "Audit");

            var profileList = await _EcosLoginService.GetAllProfilesList().ConfigureAwait(false);

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
            await GetListProfilesAsync().ConfigureAwait(false);

            if (ModelState.TryGetValue("Item.TxPassword", out var entry) && entry.ValidationState == ModelValidationState.Valid
                && ModelState.TryGetValue("Item.TxEmail", out var entry1) && entry1.ValidationState == ModelValidationState.Valid)
            {
                try
                {
                    Item.Profile = (Item.IdProfile == "1" || Item.IdProfile == "2") ? int.Parse(Item.IdProfile) : 0;

                    Item.Secret = "";
                    var response = await _EcosLoginService.AddPolicySystemUser(Item).ConfigureAwait(false);

                    if (!response.IsSuccessful)
                    {
                        TempData["warning"] = "Email already registered or invalid data provided.";
                        return Page();
                    }
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
    }
}
