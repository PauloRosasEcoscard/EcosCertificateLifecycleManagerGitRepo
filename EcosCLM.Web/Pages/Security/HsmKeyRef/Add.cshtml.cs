using EcosCLM.Application.Extensions.Security;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Web.Pages.Security.HsmKeyRef
{
    public class AddModel : BasePageModel<HsmKeyRefViewModel>
    {
        private readonly IHsmKeyRefRepository _repository;
        private readonly ILogger<AddModel> _logger;
        private readonly IHsmClusterRepository _clusterRepository;

        public SelectList HsmClusterList { get; set; } = default!;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IHsmClusterRepository clusterRepository,
            IHsmKeyRefRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
            _clusterRepository = clusterRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Item = new HsmKeyRefViewModel();
            await LoadClustersAsync().ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadClustersAsync().ConfigureAwait(false);
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _repository.ToEntity(Item);
                    entity.Id = Guid.NewGuid();
                    entity.CustomerId = CustumerId;
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;

                    await _repository.CreateAsync(entity).ConfigureAwait(false);

                    TempData["success"] = "HSM Key Reference created successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error creating HSM key reference.");
                    return Page();
                }
            }

            return Page();
        }

        private async Task LoadClustersAsync()
        {
            var clusters = await _clusterRepository.GetAll()
                .Where(x => x.CustomerId == CustumerId && x.Status == "ACTIVE")
                .ToListAsync()
                .ConfigureAwait(false);

            var clustersViewModel = _clusterRepository.ToListViewModel(clusters);

            HsmClusterList = new SelectList(clustersViewModel, "Id", "Name");
            ViewData["HsmClusterList"] = HsmClusterList;
        }
    }
}