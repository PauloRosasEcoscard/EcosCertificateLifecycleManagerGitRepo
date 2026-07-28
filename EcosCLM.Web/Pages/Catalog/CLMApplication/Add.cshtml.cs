using EcosCLM.Application.Extensions.Catalog;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Catalog;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Catalog.CLMApplication
{
    public class AddModel : BasePageModel<CLMApplicationViewModel>
    {
        private readonly ICLMApplicationRepository _repository;
        private readonly ILogger<AddModel> _logger;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICLMApplicationRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            Item = new CLMApplicationViewModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _repository.ToEntity(Item);
                    entity.Id = Guid.NewGuid();
                    entity.CustomerId = CustumerId;
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;

                    await _repository.CreateAsync(entity).ConfigureAwait(false);

                    TempData["success"] = "Application created successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error creating application.");
                    return Page();
                }
            }

            return Page();
        }
    }
}