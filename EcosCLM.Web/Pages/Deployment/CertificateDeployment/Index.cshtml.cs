using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Deployment.CertificateDeployment
{
    public class IndexModel : BasePageModel<CertificateDeploymentViewModel>
    {
        private readonly IEcosLoginService _ecosLoginService;
        private readonly ICertificateDeploymentRepository _repository;
        private readonly ILogger<IndexModel> _logger;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public CertificateDeploymentViewModel Search { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateDeploymentRepository repository)
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

            await GetData().ConfigureAwait(false);

            AddGridConfig();

            return Page();
        }

        private void AddGridConfig()
        {
            GridConfig = new GridConfiguration
            {
                Title = "Certificate Deployment",
                AddPageUrl = "Add",
                SearchPlaceholder = "Certificate",
                SearchQuery = string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new List<string>
                {
                    "Certificate",
                    "Target",
                    "Status",
                    "Deployment"
                }
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

                    Itens = Itens
                        .Skip((Pager.CurrentPage - 1) * Pager.PageSize)
                        .Take(Pager.PageSize)
                        .ToList();
                }
                else
                {
                    Itens = new List<CertificateDeploymentViewModel>();
                    Pager = new Pager(0, PageCurrent, PageSize, MaxPages);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex,
                    "Error processing certificate deployment list.");
            }
        }

        private async Task GetItens()
        {
            try
            {
                var deployments = _repository
                    .IncludingAll(new()
                    {
                        x => x.Certificate,
                        x => x.Target
                    })
                    .Where(x => x.CustomerId == CustumerId)
                    .ToList();

                Itens = _repository.ToListViewModel(deployments);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex,
                    "Error loading certificate deployments.");

                Itens = new List<CertificateDeploymentViewModel>();
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}