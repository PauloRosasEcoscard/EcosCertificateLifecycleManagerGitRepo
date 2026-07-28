using EcosCLM.Application.Extensions.Security;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Security.CertificateAuthority
{
    public class UpdateModel : BasePageModel<CertificateAuthorityViewModel>
    {
        private readonly ICertificateAuthorityRepository _repository;
        private readonly ILogger<UpdateModel> _logger;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateAuthorityRepository repository)
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
                    TempData["warning"] = "Certificate Authority not found.";
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
                _logger.LogError(ex, "Error retrieving certificate authority for update.");
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

                    existingEntity.Name = Item.Name;
                    existingEntity.ProviderType = Item.ProviderType;
                    existingEntity.BaseUrl = Item.BaseUrl;
                    existingEntity.AccountRef = Item.AccountRef;
                    existingEntity.SupportsAcme = Item.SupportsAcme;
                    existingEntity.Status = Item.Status;
                    existingEntity.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdAsync(existingEntity).ConfigureAwait(false);

                    TempData["success"] = "Certificate Authority updated successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error updating certificate authority.");
                    return Page();
                }
            }

            return Page();
        }
    }
}