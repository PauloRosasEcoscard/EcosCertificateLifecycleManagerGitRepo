using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Deployment.DeploymentTarget
{
    public class AddModel : BasePageModel<DeploymentTargetViewModel>
    {
        private readonly ILogger<AddModel> _logger;
        private readonly IDeploymentTargetRepository _repository;

        public SelectList StatusList { get; set; } = default!;
        public SelectList AutomationList { get; set; } = default!;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IDeploymentTargetRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            _logger.LogInformation("===== ONGET EXECUTADO =====");

            LoadLists();

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            _logger.LogInformation("===== ONPOSTSAVE EXECUTADO =====");

            // Campos preenchidos pelo servidor
            Item.CustomerId = CustumerId;
            Item.CreatedAt = DateTime.UtcNow;
            Item.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("CustomerId: {CustomerId}", Item.CustomerId);

            // Remove validações de campos que não vêm do formulário
            ModelState.Remove("Item.CustomerId");
            ModelState.Remove("Item.CreatedAt");
            ModelState.Remove("Item.UpdatedAt");

            if (!ModelState.IsValid)
            {
                _logger.LogError("MODELSTATE INVÁLIDO");

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError(
                            "Campo: {Campo} | Erro: {Erro}",
                            state.Key,
                            error.ErrorMessage);
                    }
                }

                LoadLists();
                return Page();
            }

            try
            {
                _logger.LogInformation("Chamando AddAsync...");

                var entity = _repository.ToEntity(Item);

                await _repository.AddAsync(entity).ConfigureAwait(false);

                _logger.LogInformation("Registro salvo com sucesso.");

                TempData["success"] = "Deployment target created successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERRO AO SALVAR");

                TempData["error"] = ex.Message;

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