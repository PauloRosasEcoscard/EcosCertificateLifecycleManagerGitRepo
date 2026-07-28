using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Integration.ApiIdempotencyKey
{
    public class DeleteModel : BasePageModel<ApiIdempotencyKeyViewModel>
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly IApiIdempotencyKeyRepository _repository;

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IApiIdempotencyKeyRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == id).ConfigureAwait(false);

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
                _logger.LogError(ex, "Error loading API idempotency key.");

                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == Item.Id).ConfigureAwait(false);

                if (entity == null)
                {
                    TempData["warning"] = "API idempotency key not found.";
                    return RedirectToPage("Index");
                }

                if (entity.CustomerId != CustumerId)
                {
                    TempData["warning"] = "Invalid operation.";
                    return RedirectToPage("Index");
                }

                await _repository.DelAsync(entity).ConfigureAwait(false);

                TempData["success"] = "API idempotency key deleted successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error deleting API idempotency key.");

                return Page();
            }
        }
    }
}