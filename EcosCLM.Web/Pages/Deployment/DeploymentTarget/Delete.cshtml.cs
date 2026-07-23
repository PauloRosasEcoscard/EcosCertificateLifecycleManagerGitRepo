using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Deployment.DeploymentTarget
{
    public class DeleteModel : BasePageModel<DeploymentTargetViewModel>
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly IDeploymentTargetRepository _repository;

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IDeploymentTargetRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == id);

                if (entity == null)
                    return RedirectToPage("Index");

                Item = _repository.ToViewModel(entity);

                if (Item.CustomerId != CustumerId)
                {
                    TempData["warning"] = "Invalid operation.";
                    return RedirectToPage("Index");
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error loading deployment target.");
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == Item.Id);

                if (entity == null)
                {
                    TempData["warning"] = "Deployment target not found.";
                    return RedirectToPage("Index");
                }

                if (entity.CustomerId != CustumerId)
                {
                    TempData["warning"] = "Invalid operation.";
                    return RedirectToPage("Index");
                }

                await _repository.DelAsync(entity);

                TempData["success"] = "Deployment target deleted successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error deleting deployment target.");

                return Page();
            }
        }
    }
}