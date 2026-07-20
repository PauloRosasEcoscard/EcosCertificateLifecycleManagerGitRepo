using EcosCLM.Application.ViewModels;
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
        private readonly ILogger<UpdModel> _logger;
        private readonly IEcosLoginService _ecosLoginService;

        public Dictionary<string, string> ProfilesDict { get; set; } = new();
        public SelectList Profiles { get; set; }

        public UpdModel(
            ILogger<UpdModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _ecosLoginService = ecosLoginService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            await LoadProfilesAsync().ConfigureAwait(false);
            await GetItens(id).ConfigureAwait(false);

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
                    await UpdateUser().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error processing user update during post.");
                }

                return RedirectToPage("Index");
            }

            await LoadProfilesAsync().ConfigureAwait(false);
            return Page();
        }

        private async Task LoadProfilesAsync()
        {
            ProfilesDict.Clear();
            ProfilesDict.Add("1", "Admin");
            ProfilesDict.Add("2", "Audit");

            var profileResult = await _ecosLoginService.GetAllProfilesList().ConfigureAwait(false);
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
            var result = await _ecosLoginService.EditPolicySystemUserProfile(Item.IdUser, Item).ConfigureAwait(false);

            if (!result.IsSuccessful)
            {
                TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
            }
        }

        private async Task GetItens(Guid id)
        {
            var result = await _ecosLoginService.GetPolicySystemUserById(id).ConfigureAwait(false);

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