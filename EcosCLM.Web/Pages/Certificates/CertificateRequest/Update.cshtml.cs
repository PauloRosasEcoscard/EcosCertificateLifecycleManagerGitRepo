using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Certificates.CertificateRequest
{
    public class UpdateModel : BasePageModel<CertificateRequestViewModel>
    {
        private readonly ICertificateRequestRepository _repository;
        private readonly ILogger<UpdateModel> _logger;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateRequestRepository repository)
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
                    TempData["warning"] = "Certificate Request not found.";
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
                _logger.LogError(ex, "Error retrieving certificate request for update.");
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

                    existingEntity.RequestType = Item.RequestType;
                    existingEntity.Status = Item.Status;
                    existingEntity.SubjectDn = Item.SubjectDn;
                    existingEntity.CertificateRequestCLMApplicationId = Item.CertificateRequestCLMApplicationId;
                    existingEntity.CertificateRequestDomainId = Item.CertificateRequestDomainId;
                    existingEntity.CertificateRequestProfileId = Item.CertificateRequestProfileId;
                    existingEntity.CaId = Item.CaId;
                    existingEntity.HsmClusterId = Item.HsmClusterId;
                    existingEntity.HsmKeyRefId = Item.HsmKeyRefId;
                    existingEntity.CsrPem = Item.CsrPem;
                    existingEntity.FailureReason = Item.FailureReason;
                    existingEntity.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdAsync(existingEntity).ConfigureAwait(false);

                    TempData["success"] = "Certificate Request updated successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error updating certificate request.");
                    return Page();
                }
            }

            return Page();
        }
    }
}