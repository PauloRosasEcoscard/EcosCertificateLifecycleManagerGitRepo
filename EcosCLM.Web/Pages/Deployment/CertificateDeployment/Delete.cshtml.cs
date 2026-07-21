using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Deployment.CertificateDeployment
{
    public class DeleteModel : BasePageModel<CertificateDeploymentViewModel>
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly ICertificateDeploymentRepository _repository;

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateDeploymentRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            await GetData(id);

            if (Item == null)
                return RedirectToPage("Index");

            if (Item.CustomerId != CustumerId)
            {
                TempData["warning"] = "Invalid operation.";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == Item.Id);

                if (entity == null)
                {
                    TempData["warning"] = "Certificate Deployment not found.";
                    return RedirectToPage("Index");
                }

                await _repository.DelAsync(entity);

                TempData["success"] = "Certificate Deployment deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error deleting Certificate Deployment.");
                return Page();
            }

            return RedirectToPage("Index");
        }

        private async Task GetData(Guid id)
        {
            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == id);

                if (entity != null)
                {
                    Item = _repository.ToViewModel(entity);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error loading Certificate Deployment.");
            }
        }
    }
}