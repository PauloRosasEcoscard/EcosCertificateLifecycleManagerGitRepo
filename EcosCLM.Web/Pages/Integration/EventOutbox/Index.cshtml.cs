using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Integration.EventOutbox
{
    public class IndexModel : BasePageModel<EventOutboxViewModel>
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IEventOutboxRepository _repository;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public EventOutboxViewModel Search { get; set; } = new();

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IEventOutboxRepository repository)
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
                Title = "Event Outbox",
                AddPageUrl = "Add",
                SearchPlaceholder = "Event Type",
                SearchQuery = Search?.EventType ?? string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new()
                {
                    "Event Type",
                    "Status",
                    "Retries",
                    "Created"
                }
            };
        }

        private async Task GetData()
        {
            try
            {
                var entities = _repository.GetAll().ToList();

                Itens = _repository.ToListViewModel(entities);

                if (!string.IsNullOrWhiteSpace(Search?.EventType))
                {
                    Itens = Itens.Where(x =>
                        x.EventType.Contains(Search.EventType,
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
                _logger.LogError(ex, "Error loading event outbox.");
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}