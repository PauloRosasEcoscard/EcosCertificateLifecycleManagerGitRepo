using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Deployment.DeploymentTarget
{
    public class IndexModel : BasePageModel<DeploymentTargetViewModel>
    {
        private readonly IEcosLoginService _ecosLoginService;
        private readonly IDeploymentTargetRepository _repository;
        private readonly ILogger<IndexModel> _logger;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public DeploymentTargetViewModel Search { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IDeploymentTargetRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _ecosLoginService = ecosLoginService;
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(int p = 1)
        {
            PageCurrent = p;

            OnPostClear(false);

            Search = GetFilters();

            await GetData();

            AddGridConfig();

            return Page();
        }

        private void AddGridConfig()
        {
            GridConfig = new GridConfiguration
            {
                Title = "Deployment Target",
                AddPageUrl = "Add",
                SearchPlaceholder = "Target Name",
                SearchQuery = Search?.Name ?? string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new List<string>
                {
                    "Name",
                    "Type",
                    "Status",
                    "Automation"
                }
            };
        }

        private async Task GetData()
        {
            try
            {
                await GetItens();

                if (Itens != null)
                {
                    int totalItems = Itens.Count();

                    Pager = new Pager(totalItems, PageCurrent, PageSize, MaxPages);

                    Itens = Itens
                        .Skip((Pager.CurrentPage - 1) * Pager.PageSize)
                        .Take(Pager.PageSize)
                        .ToList();
                }
                else
                {
                    Itens = new List<DeploymentTargetViewModel>();

                    Pager = new Pager(0, PageCurrent, PageSize, MaxPages);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex,
                    "Error processing deployment target list.");
            }
        }

        private async Task GetItens()
        {
            try
            {
                var targets = _repository
                    .FindBy(x => x.CustomerId == CustumerId)
                    .ToList();

                Itens = _repository.ToListViewModel(targets);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex,
                    "Error loading deployment targets.");

                Itens = new List<DeploymentTargetViewModel>();
            }

            await Task.CompletedTask;
        }
    }
}