using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Integration.EventOutbox
{
    public class UpdateModel : BasePageModel<EventOutboxViewModel>
    {
        private readonly ILogger<UpdateModel> _logger;
        private readonly IEventOutboxRepository _repository;

        public SelectList StatusList { get; set; } = default!;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IEventOutboxRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                LoadLists();

                var entity = await _repository.FindOneAsync(x => x.Id == id).ConfigureAwait(false);

                if (entity == null)
                    return RedirectToPage("Index");

                Item = _repository.ToViewModel(entity);

                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex, "Error loading event.");

                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadLists();
                return Page();
            }

            try
            {
                await _repository.UpdAsync(_repository.ToEntity(Item)).ConfigureAwait(false);

                TempData["success"] = "Event updated successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex, "Error updating event.");

                LoadLists();

                return Page();
            }
        }

        private void LoadLists()
        {
            StatusList = new SelectList(new[]
            {
                "PENDING",
                "PROCESSED",
                "FAILED"
            });
        }
    }
}