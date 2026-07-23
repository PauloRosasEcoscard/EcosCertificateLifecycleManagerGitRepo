using EcosCLM.Application.ViewModels;
using EcosCLM.Data.Services;
using EcosCLM.Domain.DataTypes;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EcosCLM.Web.Pages.Company.IdentityProvider
{
    public class DeleteModel : BasePageModel<AuthConfigAzureViewModel>
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly IConfiguration _configuration;

        public Dictionary<Guid, string> Customers { get; set; } = new();

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            await GetData(id).ConfigureAwait(false);

            if (Item == null)
                return RedirectToPage("Index");

            if (Item.IdCustomer != CustumerId)
            {
                TempData["warning"] = "Invalid operation";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                string url = _configuration.GetSection("AppSettings:Clients:Login").Value;
                string uri = string.Format(PolicySystemUris.deleteIdentityProvider, Item.Id);

                HttpResponseMessage response = await HttpRequestService.DeleteAsync(string.Concat(url, uri), _logger).ConfigureAwait(false);
                string responseContentHttp = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["warning"] = $"Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}.\n{responseContentHttp}|";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex.Message);
                return Page();
            }

            return RedirectToPage("Index");
        }

        private async Task GetData(int id)
        {
            try
            {
                await GetItem(id).ConfigureAwait(false);
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

            HttpResponseMessage response = await HttpRequestService.GetAsync(string.Concat(url, uri), _logger).ConfigureAwait(false);
            string responseContentHttp = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                Item = JsonConvert.DeserializeObject<AuthConfigAzureViewModel>(responseContentHttp);
            }
            else
            {
                TempData["warning"] = $"Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}.\n{responseContentHttp}|";
            }
        }
    }
}