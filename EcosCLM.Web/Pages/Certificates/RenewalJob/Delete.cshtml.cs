using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Certificates.RenewalJob
{
    public class DeleteModel : BasePageModel<RenewalJobViewModel>
    {
        private readonly IRenewalJobRepository _repository;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IRenewalJobRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                Item = await _repository.GetByIdAsync(id).ConfigureAwait(false);

                if (Item == null)
                {
                    TempData["warning"] = "Renewal Job not found.";
                    return RedirectToPage("Index");
                }

                if (Item.CustomerId != CustumerId)
                {
                    TempData["warning"] = "Unauthorized action.";
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error retrieving renewal job for deletion.");
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var existingEntity = await _repository.FindOneAsync(x => x.Id == Item.Id).ConfigureAwait(false);
                if (existingEntity == null || existingEntity.CustomerId != CustumerId)
                {
                    TempData["warning"] = "Operation not allowed.";
                    return RedirectToPage("Index");
                }

                await _repository.DelAsync(existingEntity).ConfigureAwait(false);

                TempData["success"] = "Renewal Job deleted successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error deleting renewal job.");
                return Page();
            }
        }
    }
}