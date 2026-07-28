using EcosCLM.Application.Extensions.Catalog;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Catalog.ManagedDomain
{
    public class DeleteModel : BasePageModel<ManagedDomainViewModel>
    {
        private readonly IManagedDomainRepository _repository;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IManagedDomainRepository repository)
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
                    TempData["warning"] = "Managed Domain not found.";
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
                _logger.LogError(ex, "Error retrieving managed domain for deletion.");
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

                TempData["success"] = "Managed Domain deleted successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error deleting managed domain.");
                return Page();
            }
        }
    }
}