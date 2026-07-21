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
            await LoadInitialDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadInitialDataAsync();
                return Page();
            }

            try
            {
                Item.CustomerId = CustumerId;
                Item.CreatedAt = DateTime.UtcNow;
                Item.UpdatedAt = DateTime.UtcNow;

                await _repository.AddAsync(_repository.ToEntity(Item));

                TempData["success"] = "Certificate deployment created successfully.";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                _logger.LogError(ex,
                    "Error creating certificate deployment.");

                await LoadInitialDataAsync();

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
                _logger.LogError(ex,
                    "Error loading certificate deployment auxiliary data.");
            }

            await Task.CompletedTask;
        }
    }
}