using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Deployment.DeploymentTarget
{
    public class UpdateModel : BasePageModel<DeploymentTargetViewModel>
    {
        private readonly ILogger<UpdateModel> _logger;
        private readonly IDeploymentTargetRepository _repository;

        public SelectList StatusList { get; set; } = default!;
        public SelectList AutomationList { get; set; } = default!;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IDeploymentTargetRepository repository)
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
                _logger.LogError(ex, "Error loading deployment target.");

                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Campos preenchidos pelo servidor
            Item.CustomerId = CustumerId;
            Item.UpdatedAt = DateTime.UtcNow;

            // Remove validações desses campos
            ModelState.Remove("Item.CustomerId");
            ModelState.Remove("Item.UpdatedAt");

            if (!ModelState.IsValid)
            {
                LoadLists();
                return Page();
            }

            try
            {
                var entity = await _repository.FindOneAsync(x => x.Id == Item.Id).ConfigureAwait(false);

                if (entity == null)
                {
                    TempData["warning"] = "Deployment target not found.";
                    return RedirectToPage("Index");
                }

                entity.Name = Item.Name;
                entity.TargetType = Item.TargetType;
                entity.EndpointRef = Item.EndpointRef;
                entity.SecretRef = Item.SecretRef;
                entity.AutomationEnabled = Item.AutomationEnabled;
                entity.Status = Item.Status;
                entity.CustomerId = Item.CustomerId;
                entity.UpdatedAt = Item.UpdatedAt;

                await _repository.UpdAsync(entity).ConfigureAwait(false);

                TempData["success"] = "Deployment target updated successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error updating deployment target.");

                LoadLists();

                return Page();
            }
        }

        private void LoadLists()
        {
            StatusList = new SelectList(new[]
            {
                "ACTIVE",
                "INACTIVE"
            });

            AutomationList = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Disabled",
                        Value = "0"
                    },
                    new SelectListItem
                    {
                        Text = "Enabled",
                        Value = "1"
                    }
                },
                "Value",
                "Text");

            ViewData["StatusList"] = StatusList;
            ViewData["AutomationList"] = AutomationList;
        }
    }
}