using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Integration.ApiIdempotencyKey
{
    public class UpdateModel : BasePageModel<ApiIdempotencyKeyViewModel>
    {
        private readonly ILogger<UpdateModel> _logger;
        private readonly IApiIdempotencyKeyRepository _repository;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IApiIdempotencyKeyRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
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
            // Campos preenchidos pelo servidor
            Item.CustomerId = CustumerId;

            // Remove validação de campos que não vêm do formulário
            ModelState.Remove("Item.CustomerId");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _repository.UpdAsync(_repository.ToEntity(Item)).ConfigureAwait(false);

                TempData["success"] = "API idempotency key updated successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(
                    ex,
                    "Error updating API idempotency key.");

                return Page();
            }
        }
    }
}