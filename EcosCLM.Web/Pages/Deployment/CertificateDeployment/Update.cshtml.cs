using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Deployment.CertificateDeployment
{
    public class UpdateModel : BasePageModel<CertificateDeploymentViewModel>
    {
        private readonly IEcosLoginService _ecosLoginService;
        private readonly ILogger<UpdateModel> _logger;
        private readonly ICertificateDeploymentRepository _repository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IDeploymentTargetRepository _targetRepository;

        public SelectList Certificates { get; set; } = default!;
        public SelectList Targets { get; set; } = default!;
        public SelectList StatusList { get; set; } = default!;

        public UpdateModel(
            ILogger<UpdateModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateDeploymentRepository repository,
            ICertificateRepository certificateRepository,
            IDeploymentTargetRepository targetRepository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _ecosLoginService = ecosLoginService;
            _repository = repository;
            _certificateRepository = certificateRepository;
            _targetRepository = targetRepository;
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

                await LoadInitialDataAsync().ConfigureAwait(false);

                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(
                    ex,
                    "Error loading certificate deployment.");

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
                await LoadInitialDataAsync().ConfigureAwait(false);
                return Page();
            }

            try
            {
                await _repository.UpdAsync(_repository.ToEntity(Item)).ConfigureAwait(false);

                TempData["success"] = "Certificate deployment updated successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(
                    ex,
                    "Error updating certificate deployment.");

                await LoadInitialDataAsync().ConfigureAwait(false);

                return Page();
            }
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                var certificates = _certificateRepository
                    .FindBy(x => x.CustomerId == CustumerId)
                    .ToList();

                Certificates = new SelectList(
                    certificates,
                    "Id",
                    "SubjectDn",
                    Item.CertificateId);

                var targets = _targetRepository
                    .FindBy(x => x.CustomerId == CustumerId)
                    .ToList();

                Targets = new SelectList(
                    targets,
                    "Id",
                    "Name",
                    Item.TargetId);

                StatusList = new SelectList(
                    new[]
                    {
                        "PENDING",
                        "SUCCESS",
                        "FAILED"
                    },
                    Item.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading auxiliary data.");
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}