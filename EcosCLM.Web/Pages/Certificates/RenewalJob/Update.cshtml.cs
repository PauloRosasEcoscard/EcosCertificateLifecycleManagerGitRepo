using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Certificates.RenewalJob
{
    public class UpdateModel : BasePageModel<RenewalJobViewModel>
    {
        private readonly IRenewalJobRepository _repository;
        private readonly ILogger<UpdateModel> _logger;

        public UpdateModel(
            ILogger<UpdateModel> logger,
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
                _logger.LogError(ex, "Error retrieving renewal job for update.");
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingEntity = await _repository.FindOneAsync(x => x.Id == Item.Id).ConfigureAwait(false);
                    if (existingEntity == null || existingEntity.CustomerId != CustumerId)
                    {
                        TempData["warning"] = "Operation not allowed.";
                        return RedirectToPage("Index");
                    }

                    existingEntity.CertificateId = Item.CertificateId;
                    existingEntity.ScheduledAt = Item.ScheduledAt;
                    existingEntity.DueAt = Item.DueAt;
                    existingEntity.Status = Item.Status;
                    existingEntity.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdAsync(existingEntity).ConfigureAwait(false);

                    TempData["success"] = "Renewal Job updated successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error updating renewal job.");
                    return Page();
                }
            }

            return Page();
        }
    }
}