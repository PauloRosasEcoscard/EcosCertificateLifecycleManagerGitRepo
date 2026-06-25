using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Company.Users
{
    public class UpdModel : BasePageModel<PolicySystemUserViewModel>
    {
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ILogger<UpdModel> _logger;
        private readonly ISyslogService _syslogService;
        private readonly IEcosLoginService _ecosLoginService;

        public Dictionary<string, string> ProfilesDict { get; set; } = new();
        public SelectList Profiles { get; set; }

        public UpdModel(
            ILogger<UpdModel> logger,
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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            await LoadProfilesAsync();
            await GetItens(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.TryGetValue("Item.TxPassword", out var entry) && entry.ValidationState == ModelValidationState.Valid
                && ModelState.TryGetValue("Item.TxEmail", out var entry1) && entry1.ValidationState == ModelValidationState.Valid)
            {
                try
                {
                    Item.Profile = (Item.IdProfile == "1" || Item.IdProfile == "2") ? int.Parse(Item.IdProfile) : 0;
                    await UpdateUser();
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error processing user update during post.");
                }

                return RedirectToPage("Index");
            }

            await LoadProfilesAsync();
            return Page();
        }

        private async Task LoadProfilesAsync()
        {
            ProfilesDict.Clear();
            ProfilesDict.Add("1", "Admin");
            ProfilesDict.Add("2", "Audit");

            var profileResult = await _ecosLoginService.GetAllProfilesList();
            if (profileResult.IsSuccessful && profileResult.Data != null)
            {
                var dbProfiles = profileResult.Data
                    .Where(x => x.TxTitle != "Admin")
                    .ToDictionary(x => x.IdProfile, x => x.TxTitle);

                foreach (var dbProfile in dbProfiles)
                {
                    ProfilesDict.Add(dbProfile.Key.ToString(), dbProfile.Value);
                }
            }

            Profiles = new SelectList(ProfilesDict, "Key", "Value");
        }

        private async Task UpdateUser()
        {
            var result = await _ecosLoginService.EditPolicySystemUserProfile(Item.IdUser, Item);

            if (result.IsSuccessful)
            {
                _auditLogs.Create(new AuditLogs
                {
                    Date = DateTime.Now,
                    User = Email,
                    IdCustumer = CustumerId,
                    Log = $"User: {Email} Update user",
                    LogType = "Company Management"
                }, _syslogService, HttpContextAccessor);
            }
            else
            {
                TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
            }
        }

        private async Task GetItens(Guid id)
        {
            var result = await _ecosLoginService.GetPolicySystemUserById(id);

            if (result.IsSuccessful && result.Data != null)
            {
                Item = result.Data;
            }
            else
            {
                TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
            }
        }
    }
}