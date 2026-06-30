using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.ViewModels;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Company.PolicySettings
{
    public class IndexModel : BasePageModel<PolicySettingsViewModel>
    {
        private readonly IPolicySettingsRepository _policySettingsRepository;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration config,
            IPolicySettingsRepository policySettingsRepository,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _policySettingsRepository = policySettingsRepository;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = _policySettingsRepository.GetByIdCustumer(CustumerId);

            if (Item == null)
            {
                Item = _policySettingsRepository.Create(new PolicySettingsViewModel
                {
                    CustumerId = CustumerId,
                    TimeoutSession = 30
                });

                return RedirectToPage("Index");
            }

            if (Item.CustumerId != CustumerId)
            {
                TempData["warning"] = "Invalid operation";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (ModelState.IsValid)
            {
                _policySettingsRepository.Edit(Item);
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}