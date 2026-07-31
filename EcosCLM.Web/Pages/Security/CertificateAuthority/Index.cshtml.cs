using EcosCLM.Application.Extensions.Security;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Security.CertificateAuthority
{
    public class IndexModel : BasePageModel<CertificateAuthorityViewModel>
    {
        private readonly ICertificateAuthorityRepository _repository;
        private readonly ILogger<IndexModel> _logger;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public CertificateAuthorityViewModel Search { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateAuthorityRepository repository)
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
                Title = "Certificate Authorities",
                AddPageUrl = "Add",
                SearchPlaceholder = "Name or Provider Type",
                SearchQuery = Search?.Name ?? string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new List<string> { "Name", "Provider Type", "ACME", "Status", "Created At" }
            };
        }

        private async Task GetData()
        {
            try
            {
                // Envia apenas o termo de busca em vez do JSON serializado
                var searchTerm = Search?.Name;

                var listResult = await _repository.GetAllWithPageAsync(
                    page: PageSize,
                    offset: (PageCurrent - 1) * PageSize,
                    filter: searchTerm,
                    oderBy: "name",
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
                    Itens = new List<CertificateAuthorityViewModel>();
                    Pager = new Pager(0, PageCurrent, PageSize, MaxPages);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error processing certificate authority list data.");
            }
        }
    }
}