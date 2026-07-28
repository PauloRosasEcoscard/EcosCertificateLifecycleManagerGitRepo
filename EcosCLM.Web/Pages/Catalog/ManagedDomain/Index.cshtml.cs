using EcosCLM.Application.Extensions.Catalog;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Catalog.ManagedDomain
{
    public class IndexModel : BasePageModel<ManagedDomainViewModel>
    {
        private readonly IManagedDomainRepository _repository;
        private readonly ILogger<IndexModel> _logger;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public ManagedDomainViewModel Search { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IManagedDomainRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
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
                Title = "Managed Domains",
                AddPageUrl = "Add",
                SearchPlaceholder = "FQDN or Status",
                SearchQuery = Search?.Fqdn ?? string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new List<string> { "FQDN", "Status", "Validated At", "Expires At", "Created At" }
            };
        }

        private async Task GetData()
        {
            try
            {
                var filterJson = Search != null ? Newtonsoft.Json.JsonConvert.SerializeObject(Search) : null;
                var listResult = await _repository.GetAllWithPageAsync(
                    page: PageSize,
                    offset: (PageCurrent - 1) * PageSize,
                    filter: filterJson,
                    oderBy: "fqdn",
                    customer: CustumerId
                ).ConfigureAwait(false);

                if (listResult != null)
                {
                    int totalItems = listResult.Count();
                    Pager = new Pager(totalItems, PageCurrent, PageSize, MaxPages);
                    Itens = listResult;
                }
                else
                {
                    Itens = new List<ManagedDomainViewModel>();
                    Pager = new Pager(0, PageCurrent, PageSize, MaxPages);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error processing managed domain list data.");
            }
        }
    }
}