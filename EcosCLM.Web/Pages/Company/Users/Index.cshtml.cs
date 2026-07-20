using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Company.Users
{
    public class IndexModel : BasePageModel<PolicySystemUserViewModel>
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IEcosLoginService _ecosLoginService;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search", SupportsGet = true)]
        public PolicySystemUserViewModel Search { get; set; }

        // Dicionários auxiliares para carregar dados que não pertencem à ViewModel
        public Dictionary<Guid, string> ProfileTitles { get; set; } = new();

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration config,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _ecosLoginService = ecosLoginService;
        }

        public async Task<IActionResult> OnGetAsync(int p = 1)
        {
            PageCurrent = p;
            OnPostClear(false);

            Search = GetFilters();

            await GetData().ConfigureAwait(false);

            AddGridConfig();

            return Page();
        }

        private void AddGridConfig()
        {
            GridConfig = new GridConfiguration
            {
                Title = "Users",
                AddPageUrl = "Add",
                SearchPlaceholder = "Name",
                SearchQuery = Search?.TxName ?? string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new List<string> { "Name", "Email", "Profile Type", "2FA" }
            };
        }

        private async Task GetData()
        {
            try
            {
                await GetItens().ConfigureAwait(false);

                if (Itens != null && Itens.Any())
                {
                    if (!string.IsNullOrEmpty(Search?.TxName))
                    {
                        Itens = Itens.Where(x => x.TxName != null && x.TxName.Contains(Search.TxName, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    var usersList = Itens.ToList();
                    ProfileTitles.Clear();

                    var enrichmentTasks = usersList.Select(async user =>
                    {
                        string title = await ResolveProfileTitleAsync(user.Profile, user.TxEmail).ConfigureAwait(false);
                        lock (ProfileTitles)
                        {
                            ProfileTitles[user.IdUser] = title;
                        }

                        var blockResult = await _ecosLoginService.GetUserIsBlocked(user).ConfigureAwait(false);
                        user.IsBlocked = blockResult.IsSuccessful && blockResult.Data;
                    });

                    await Task.WhenAll(enrichmentTasks).ConfigureAwait(false);

                    Pager = new Pager(usersList.Count, PageCurrent, PageSize, MaxPages);
                    Itens = usersList.Skip((Pager.CurrentPage - 1) * Pager.PageSize).Take(Pager.PageSize).ToList();
                }
                else
                {
                    Itens = new List<PolicySystemUserViewModel>();
                    Pager = new Pager(0, PageCurrent, PageSize, MaxPages);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error fetching and aggregating user data.");
            }
        }

        private async Task GetItens()
        {
            var result = await _ecosLoginService.GetPolicySystemCompanyUsers(CustumerId).ConfigureAwait(false);

            if (result.IsSuccessful && result.Data != null)
            {
                Itens = result.Data;
            }
            else
            {
                TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
                Itens = new List<PolicySystemUserViewModel>();
            }
        }

        private async Task<string> ResolveProfileTitleAsync(int profileType, string email)
        {
            if (profileType == 1) return "Admin";
            if (profileType == 2) return "Audit";

            var response = await _ecosLoginService.GetProfile(email).ConfigureAwait(false);
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Data))
            {
                return response.Data;
            }

            return "Custom";
        }

        public async Task<IActionResult> OnPostReset2FAAsync(Guid id)
        {
            var userResult = await _ecosLoginService.GetPolicySystemUserById(id).ConfigureAwait(false);
            if (userResult.IsSuccessful && userResult.Data != null)
            {
                var user = userResult.Data;
                user.Secret = "";

                var updateResult = await _ecosLoginService.EditPolicySystemUserProfile(user.IdUser, user).ConfigureAwait(false);
                TempData[updateResult.IsSuccessful ? "success" : "warning"] = updateResult.IsSuccessful ? "2FA reseted successfully" : "Oops, something happened, please try again!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDisable2FAAsync(Guid id)
        {
            var userResult = await _ecosLoginService.GetPolicySystemUserById(id).ConfigureAwait(false);
            if (userResult.IsSuccessful && userResult.Data != null)
            {
                var user = userResult.Data;
                user.Auth2fa = false;

                var updateResult = await _ecosLoginService.EditPolicySystemUserProfile(user.IdUser, user).ConfigureAwait(false);
                TempData[updateResult.IsSuccessful ? "success" : "warning"] = updateResult.IsSuccessful ? "2FA Disabled successfully" : "Oops, something happened, please try again!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnlockUserAsync(Guid id)
        {
            var userResult = await _ecosLoginService.GetPolicySystemUserById(id).ConfigureAwait(false);
            if (userResult.IsSuccessful && userResult.Data != null)
            {
                var user = userResult.Data;
                user.FailedAccessAttempts = 0;

                var updateResult = await _ecosLoginService.EditPolicySystemUserProfile(user.IdUser, user).ConfigureAwait(false);
                TempData[updateResult.IsSuccessful ? "success" : "warning"] = updateResult.IsSuccessful ? "User unlocked successfully" : "Oops, something happened, please try again!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEnable2FAAsync(Guid id)
        {
            var userResult = await _ecosLoginService.GetPolicySystemUserById(id).ConfigureAwait(false);
            if (userResult.IsSuccessful && userResult.Data != null)
            {
                var user = userResult.Data;
                user.Auth2fa = true;

                var updateResult = await _ecosLoginService.EditPolicySystemUserProfile(user.IdUser, user).ConfigureAwait(false);
                TempData[updateResult.IsSuccessful ? "success" : "warning"] = updateResult.IsSuccessful ? "2FA Enabled successfully" : "Oops, something happened, please try again!";
            }

            return RedirectToPage();
        }
    }
}