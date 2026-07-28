using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Certificates.RenewalJob
{
    public class IndexModel : BasePageModel<RenewalJobViewModel>
    {
        private readonly IRenewalJobRepository _repository;
        private readonly ILogger<IndexModel> _logger;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public RenewalJobViewModel Search { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IRenewalJobRepository repository)
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
                Title = "Renewal Jobs",
                AddPageUrl = "Add",
                SearchPlaceholder = "Status or Certificate ID",
                SearchQuery = Search?.Status ?? string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new List<string> { "Certificate ID", "Scheduled At", "Due At", "Status", "Created At" }
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
                    oderBy: "due",
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
                    Itens = new List<RenewalJobViewModel>();
                    Pager = new Pager(0, PageCurrent, PageSize, MaxPages);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error processing renewal job list data.");
            }
        }
    }
}