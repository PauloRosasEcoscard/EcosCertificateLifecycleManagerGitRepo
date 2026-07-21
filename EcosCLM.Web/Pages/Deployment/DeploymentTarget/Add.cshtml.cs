using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Deployment.DeploymentTarget
{
    public class AddModel : BasePageModel<DeploymentTargetViewModel>
    {
        private readonly ILogger<AddModel> _logger;
        private readonly IDeploymentTargetRepository _repository;

        public SelectList StatusList { get; set; } = default!;
        public SelectList AutomationList { get; set; } = default!;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IDeploymentTargetRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            LoadLists();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadLists();
                return Page();
            }

            try
            {
                Item.CustomerId = CustumerId;
                Item.CreatedAt = DateTime.UtcNow;
                Item.UpdatedAt = DateTime.UtcNow;

                await _repository.AddAsync(_repository.ToEntity(Item));

                TempData["success"] = "Deployment target created successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error creating deployment target.");

                LoadLists();
                return Page();
            }
        }

        private void LoadLists()
        {
            StatusList = new SelectList(new[]
            {
                "ACTIVE",
                "INACTIVE"
            });

            AutomationList = new SelectList(
                new List<SelectListItem>
                {
                    new("Disabled", "0"),
                    new("Enabled", "1")
                },
                "Value",
                "Text");

            ViewData["StatusList"] = StatusList;
            ViewData["AutomationList"] = AutomationList;
        }
    }
}