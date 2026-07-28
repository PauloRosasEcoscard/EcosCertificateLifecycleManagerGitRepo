using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Integration.ApiIdempotencyKey
{
    public class IndexModel : BasePageModel<ApiIdempotencyKeyViewModel>
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiIdempotencyKeyRepository _repository;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public ApiIdempotencyKeyViewModel Search { get; set; } = new();

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IApiIdempotencyKeyRepository repository)
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
                Title = "API Idempotency Keys",
                AddPageUrl = "Add",
                SearchPlaceholder = "Key",
                SearchQuery = Search?.Key ?? string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new()
                {
                    "Key",
                    "Expires At",
                    "Response"
                }
            };
        }

        private async Task GetData()
        {
            try
            {
                var entities = _repository
                    .FindBy(x => x.CustomerId == CustumerId)
                    .ToList();

                Itens = _repository.ToListViewModel(entities);

                if (!string.IsNullOrWhiteSpace(Search?.Key))
                {
                    Itens = Itens.Where(x =>
                        x.Key.Contains(Search.Key,
                        StringComparison.OrdinalIgnoreCase));
                }

                int totalItems = Itens.Count();

                Pager = new Pager(totalItems, PageCurrent, PageSize, MaxPages);

                Itens = Itens
                    .Skip((Pager.CurrentPage - 1) * Pager.PageSize)
                    .Take(Pager.PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex,
                    "Error loading api idempotency keys.");
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}