using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Company.IdentityProvider
{
    public class IndexModel : BasePageModel<AuthConfigAzureViewModel>
    {
        private readonly IEcosLoginService _ecosLoginService;
        private readonly ILogger<IndexModel> _logger;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public AuthConfigAzureViewModel Search { get; set; }

        public Dictionary<Guid, string> Customers { get; set; } = new();
        public string UrlForCustomer { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _ecosLoginService = ecosLoginService;
        }

        public async Task<IActionResult> OnGetAsync(int p = 1)
        {
            PageCurrent = p;
            OnPostClear(false);

            Search = GetFilters();

            await LoadCustomersAsync().ConfigureAwait(false);

            UrlForCustomer = GetUrlForCustomer();

            await GetData().ConfigureAwait(false);

            AddGridConfig();
            return Page();
        }

        private void AddGridConfig()
        {
            GridConfig = new GridConfiguration
            {
                Title = "Identity Provider",
                AddPageUrl = "Add",
                SearchPlaceholder = "Name",
                SearchQuery = Search?.Name ?? string.Empty,
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

                if (Itens != null)
                {
                    int totalItems = Itens.Count();
                    Pager = new Pager(totalItems, PageCurrent, PageSize, MaxPages);
                    Itens = Itens.Skip((Pager.CurrentPage - 1) * Pager.PageSize).Take(Pager.PageSize).ToList();
                }
                else
                {
                    Itens = new List<AuthConfigAzureViewModel>();
                    Pager = new Pager(0, PageCurrent, PageSize, MaxPages);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error processing identity provider list data.");
            }
        }

        private async Task GetItens()
        {
            var result = await _ecosLoginService.GetAuthConfigByCustomerId(CustumerId).ConfigureAwait(false);

            if (result.IsSuccessful && result.Data != null)
            {
                Itens = result.Data;
            }
            else
            {
                TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
                Itens = new List<AuthConfigAzureViewModel>();
            }
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var result = await _ecosLoginService.GetAllCustomers().ConfigureAwait(false);
                if (result.IsSuccessful && result.Data != null)
                {
                    Customers = result.Data.ToDictionary(x => x.IdCustomer, x => x.TxTitle);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers list for Identity Provider index.");
            }
        }

        private string GetUrlForCustomer()
        {
            var urlBase = $"{Request.Scheme}://{Request.Host}";

            if (Customers.TryGetValue(CustumerId, out var customerName))
            {
                return $"{urlBase}/Index?customer={Uri.EscapeDataString(customerName)}";
            }

            return $"{urlBase}/Index";
        }
    }
}