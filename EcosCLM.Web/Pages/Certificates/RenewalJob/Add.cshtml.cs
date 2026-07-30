using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Web.Pages.Certificates.RenewalJob
{
    public class AddModel : BasePageModel<RenewalJobViewModel>
    {
        private readonly IRenewalJobRepository _repository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<AddModel> _logger;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IRenewalJobRepository repository,
            ICertificateRepository certificateRepository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
            _certificateRepository = certificateRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Item = new RenewalJobViewModel
            {
                Status = "SCHEDULED",
                ScheduledAt = DateTime.UtcNow,
                DueAt = DateTime.UtcNow.AddDays(30)
            };

            await LoadCertificates().ConfigureAwait(false);

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

                    TempData["success"] = "Renewal Job created successfully!";

                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error creating renewal job.");

                    await LoadCertificates().ConfigureAwait(false);

                    return Page();
                }
            }

            await LoadCertificates().ConfigureAwait(false);

            return Page();
        }

        private async Task LoadCertificates()
        {
            try
            {
                var result = await _certificateRepository.GetAll()
                    .Where(x => x.CustomerId == CustumerId)
                    .OrderBy(x => x.SubjectDn)
                    .ToListAsync()
                    .ConfigureAwait(false);

                ViewData["Certificates"] = _certificateRepository.ToListViewModel(result);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error loading certificates.");

                ViewData["Certificates"] = new List<CertificateViewModel>();
            }
        }
    }
}