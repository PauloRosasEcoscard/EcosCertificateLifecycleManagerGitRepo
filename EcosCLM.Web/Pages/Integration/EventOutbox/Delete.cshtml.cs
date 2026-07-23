using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Integration.EventOutbox
{
    public class DeleteModel : BasePageModel<EventOutboxViewModel>
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly IEventOutboxRepository _repository;

        public DeleteModel(
            ILogger<DeleteModel> logger,
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
                var entity = await _repository.FindOneAsync(x => x.Id == id);

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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == Item.Id);

                if (entity == null)
                {
                    TempData["warning"] = "Event not found.";
                    return RedirectToPage("Index");
                }

                await _repository.DelAsync(entity);

                TempData["success"] = "Event deleted successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error deleting event.");

                return Page();
            }
        }
    }
}