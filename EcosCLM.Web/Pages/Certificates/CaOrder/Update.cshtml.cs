using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Certificates.CaOrder
{
    public class UpdateModel : BasePageModel<CaOrderViewModel>
    {
        private readonly ICaOrderRepository _repository;
        private readonly ILogger<UpdateModel> _logger;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICaOrderRepository repository)
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
                    TempData["warning"] = "CA Order not found.";
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
                _logger.LogError(ex, "Error retrieving CA order for update.");
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

                    existingEntity.RequestId = Item.RequestId;
                    existingEntity.CaId = Item.CaId;
                    existingEntity.ExternalOrderId = Item.ExternalOrderId;
                    existingEntity.ExternalCertificateId = Item.ExternalCertificateId;
                    existingEntity.Status = Item.Status;
                    existingEntity.ErrorCode = Item.ErrorCode;
                    existingEntity.ErrorMessage = Item.ErrorMessage;
                    existingEntity.RawResponseRef = Item.RawResponseRef;

                    if (Item.Status == "SUBMITTED" && !existingEntity.SubmittedAt.HasValue)
                    {
                        existingEntity.SubmittedAt = DateTime.UtcNow;
                    }
                    if ((Item.Status == "COMPLETED" || Item.Status == "FAILED") && !existingEntity.CompletedAt.HasValue)
                    {
                        existingEntity.CompletedAt = DateTime.UtcNow;
                    }

                    existingEntity.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdAsync(existingEntity).ConfigureAwait(false);

                    TempData["success"] = "CA Order updated successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error updating CA order.");
                    return Page();
                }
            }

            return Page();
        }
    }
}