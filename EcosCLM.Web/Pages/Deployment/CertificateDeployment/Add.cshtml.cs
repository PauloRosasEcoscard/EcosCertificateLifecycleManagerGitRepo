using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Deployment;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Deployment.CertificateDeployment
{
    public class AddModel : BasePageModel<CertificateDeploymentViewModel>
    {
        private readonly IEcosLoginService _ecosLoginService;
        private readonly ILogger<AddModel> _logger;
        private readonly ICertificateDeploymentRepository _repository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IDeploymentTargetRepository _targetRepository;

        public SelectList Certificates { get; set; } = default!;
        public SelectList Targets { get; set; } = default!;
        public SelectList StatusList { get; set; } = default!;

        public AddModel(
            ILogger<AddModel> logger,
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

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadInitialDataAsync().ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Item.CustomerId = CustumerId;
            Item.CreatedAt = DateTime.UtcNow;
            Item.UpdatedAt = DateTime.UtcNow;

            ModelState.Remove("Item.CustomerId");
            ModelState.Remove("Item.CreatedAt");
            ModelState.Remove("Item.UpdatedAt");

            if (!ModelState.IsValid)
            {
                await LoadInitialDataAsync().ConfigureAwait(false);
                return Page();
            }

            try
            {
                await _repository.AddAsync(_repository.ToEntity(Item)).ConfigureAwait(false);

                TempData["success"] = "Certificate deployment created successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex,
                    "Error creating certificate deployment.");

                await LoadInitialDataAsync().ConfigureAwait(false);

                return Page();
            }
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                _logger.LogInformation("========== LOAD INITIAL DATA ==========");
                _logger.LogInformation("CustumerId: {CustomerId}", CustumerId);

                // CERTIFICATES

                var allCertificates = _certificateRepository
                    .FindBy(x => true)
                    .ToList();

                _logger.LogInformation("Total Certificates (sem filtro): {Count}", allCertificates.Count);

                foreach (var certificate in allCertificates)
                {
                    _logger.LogInformation(
                        "Certificate -> Id={Id} Subject={Subject} CustomerId={CustomerId}",
                        certificate.Id,
                        certificate.SubjectDn,
                        certificate.CustomerId);
                }

                var certificates = _certificateRepository
                    .FindBy(x => x.CustomerId == CustumerId)
                    .ToList();

                _logger.LogInformation("Certificates encontrados: {Count}", certificates.Count);

                Certificates = new SelectList(
                    certificates,
                    "Id",
                    "SubjectDn",
                    Item.CertificateId);

                // TARGETS

                var allTargets = _targetRepository
                    .FindBy(x => true)
                    .ToList();

                _logger.LogInformation("Total Targets (sem filtro): {Count}", allTargets.Count);

                foreach (var target in allTargets)
                {
                    _logger.LogInformation(
                        "Target -> Id={Id} Name={Name} CustomerId={CustomerId}",
                        target.Id,
                        target.Name,
                        target.CustomerId);
                }

                var targets = _targetRepository
                    .FindBy(x => x.CustomerId == CustumerId)
                    .ToList();

                _logger.LogInformation("Targets encontrados: {Count}", targets.Count);

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

                ViewData["Certificates"] = Certificates;
                ViewData["Targets"] = Targets;
                ViewData["StatusList"] = StatusList;

                _logger.LogInformation("ViewData preenchido com sucesso.");
                _logger.LogInformation("=======================================");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error loading certificate deployment auxiliary data.");
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}