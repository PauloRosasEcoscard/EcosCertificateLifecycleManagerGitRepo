using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Integration;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Integration.EventOutbox
{
    public class AddModel : BasePageModel<EventOutboxViewModel>
    {
        private readonly ILogger<AddModel> _logger;
        private readonly IEventOutboxRepository _repository;

        public SelectList StatusList { get; set; } = default!;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IEventOutboxRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            LoadLists();

            Item.Status = "PENDING";
            Item.CreatedAt = DateTime.UtcNow;
            Item.Retries = 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadLists();
                return Page();
            }

            try
            {
                if (Item.Id == Guid.Empty)
                    Item.Id = Guid.NewGuid();

                await _repository.AddAsync(_repository.ToEntity(Item)).ConfigureAwait(false);

                TempData["success"] = "Event created successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error creating event.");

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