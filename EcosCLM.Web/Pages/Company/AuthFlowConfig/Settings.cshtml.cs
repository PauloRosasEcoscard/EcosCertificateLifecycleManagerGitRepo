using EcosCLM.Application.ViewModels;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Company.AuthFlowConfig
{
    public class SettingsModel : BasePageModel<AuthFlowConfigViewModel>
    {
        private readonly ILogger<SettingsModel> _logger;
        private readonly IEcosLoginService _ecosLoginService;

        public Dictionary<Guid, string> Customers { get; set; }

        public SettingsModel(
            ILogger<SettingsModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _ecosLoginService = ecosLoginService;

            Customers = new Dictionary<Guid, string>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCustomersAsync();
            await GetData();

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

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await UpdateAuthFlowConfig();
                    return Page();
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error processing configuration update during post.");
                    return Page();
                }
            }
            return Page();
        }

        private async Task GetData()
        {
            try
            {
                var result = await _ecosLoginService.GetAuthFlowConfigByCustomerId(CustumerId);

                if (result.IsSuccessful && result.Data != null)
                {
                    Item = result.Data;
                }
                else
                {
                    TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error retrieving data for Customer ID: {CustomerId}", CustumerId);
            }
        }

        private async Task UpdateAuthFlowConfig()
        {
            Item.IdCustomer = CustumerId;

            var result = await _ecosLoginService.EditClientAuthFlowConfig(CustumerId, Item);

            if (result.IsSuccessful)
            {
                TempData["success"] = "Authentication Flow updated successfully!";
            }
            else
            {
                TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
            }
        }
    }
}