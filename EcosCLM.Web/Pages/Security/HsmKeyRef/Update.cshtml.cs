using EcosCLM.Application.Extensions.Security;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Security.HsmKeyRef
{
    public class UpdateModel : BasePageModel<HsmKeyRefViewModel>
    {
        private readonly IHsmKeyRefRepository _repository;
        private readonly ILogger<UpdateModel> _logger;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IHsmKeyRefRepository repository)
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
                    TempData["warning"] = "HSM Key Reference not found.";
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
                _logger.LogError(ex, "Error retrieving HSM key reference for update.");
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

                    existingEntity.HsmClusterId = Item.HsmClusterId;
                    existingEntity.KeyLabel = Item.KeyLabel;
                    existingEntity.KeyHandle = Item.KeyHandle;
                    existingEntity.Algorithm = Item.Algorithm;
                    existingEntity.KeySize = Item.KeySize;
                    existingEntity.CurveName = Item.CurveName;
                    existingEntity.Extractable = Item.Extractable;
                    existingEntity.Status = Item.Status;
                    existingEntity.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdAsync(existingEntity).ConfigureAwait(false);

                    TempData["success"] = "HSM Key Reference updated successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error updating HSM key reference.");
                    return Page();
                }
            }

            return Page();
        }
    }
}