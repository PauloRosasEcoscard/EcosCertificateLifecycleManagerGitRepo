using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Integration.ApiIdempotencyKey
{
    public class AddModel : BasePageModel<ApiIdempotencyKeyViewModel>
    {
        private readonly ILogger<AddModel> _logger;
        private readonly IApiIdempotencyKeyRepository _repository;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IApiIdempotencyKeyRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            Item.ExpiresAt = DateTime.UtcNow.AddDays(1);

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            // Campos preenchidos pelo servidor
            Item.CustomerId = CustumerId;

            // Remove a validação desse campo, pois ele não vem do formulário
            ModelState.Remove("Item.CustomerId");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (Item.Id == Guid.Empty)
                    Item.Id = Guid.NewGuid();

                await _repository.AddAsync(_repository.ToEntity(Item));

                TempData["success"] = "API idempotency key created successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(
                    ex,
                    "Error creating API idempotency key.");

                return Page();
            }
        }
    }
}