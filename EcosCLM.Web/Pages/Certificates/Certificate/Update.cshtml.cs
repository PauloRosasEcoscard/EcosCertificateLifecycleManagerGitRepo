using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Certificates.Certificate
{
    public class UpdateModel : BasePageModel<CertificateViewModel>
    {
        private readonly ICertificateRepository _repository;
        private readonly ILogger<UpdateModel> _logger;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateRepository repository)
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
                    TempData["warning"] = "Certificate not found.";
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
                _logger.LogError(ex, "Error retrieving certificate for update.");
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

                    existingEntity.SerialNumber = Item.SerialNumber;
                    existingEntity.ThumbprintSha256 = Item.ThumbprintSha256;
                    existingEntity.SubjectDn = Item.SubjectDn;
                    existingEntity.IssuerDn = Item.IssuerDn;
                    existingEntity.NotBefore = Item.NotBefore;
                    existingEntity.NotAfter = Item.NotAfter;
                    existingEntity.CertificatePem = Item.CertificatePem;
                    existingEntity.ChainPem = Item.ChainPem;
                    existingEntity.Status = Item.Status;
                    existingEntity.RevocationReason = Item.RevocationReason;

                    if (Item.Status == "REVOKED" && !existingEntity.RevokedAt.HasValue)
                    {
                        existingEntity.RevokedAt = DateTime.UtcNow;
                    }

                    existingEntity.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdAsync(existingEntity).ConfigureAwait(false);

                    TempData["success"] = "Certificate updated successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error updating certificate.");
                    return Page();
                }
            }

            return Page();
        }
    }
}