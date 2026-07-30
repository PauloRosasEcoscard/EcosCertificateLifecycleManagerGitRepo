using EcosCLM.Application.Extensions.Catalog;
using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Web.Pages.Certificates.CertificateRequest
{
    public class AddModel : BasePageModel<CertificateRequestViewModel>
    {
        private readonly ICertificateRequestRepository _repository;
        private readonly ICLMApplicationRepository _applicationRepository;
        private readonly IManagedDomainRepository _managedDomainRepository;
        private readonly ICertificateProfileRepository _profileRepository;
        private readonly ICertificateAuthorityRepository _certificateAuthorityRepository;
        private readonly IHsmClusterRepository _hsmClusterRepository;

        private readonly ILogger<AddModel> _logger;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateRequestRepository repository,
            ICLMApplicationRepository applicationRepository,
            IManagedDomainRepository managedDomainRepository,
            ICertificateProfileRepository profileRepository,
            ICertificateAuthorityRepository certificateAuthorityRepository,
            IHsmClusterRepository hsmClusterRepository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
            _applicationRepository = applicationRepository;
            _managedDomainRepository = managedDomainRepository;
            _profileRepository = profileRepository;
            _certificateAuthorityRepository = certificateAuthorityRepository;
            _hsmClusterRepository = hsmClusterRepository;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            Item = new CertificateRequestViewModel
            {
                Status = "DRAFT"
            };

            await LoadCombos().ConfigureAwait(false);

            return Page();
        }


        private async Task LoadCombos()
        {
            try
            {
                ViewData["Applications"] = await _applicationRepository
                    .GetAll()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);


                ViewData["Domains"] = await _managedDomainRepository
                    .GetAll()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Fqdn
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);


                ViewData["Profiles"] = await _profileRepository
                    .GetAll()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);


                ViewData["Authorities"] = await _certificateAuthorityRepository
                    .GetAll()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);


                ViewData["HsmClusters"] = await _hsmClusterRepository
                    .GetAll()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading certificate request combos.");

                ViewData["Applications"] = new List<SelectListItem>();
                ViewData["Domains"] = new List<SelectListItem>();
                ViewData["Profiles"] = new List<SelectListItem>();
                ViewData["Authorities"] = new List<SelectListItem>();
                ViewData["HsmClusters"] = new List<SelectListItem>();
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCombos().ConfigureAwait(false);

                return Page();
            }

            try
            {
                var entity = _repository.ToEntity(Item);

                entity.Id = Guid.NewGuid();
                entity.CustomerId = CustumerId;
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.CreateAsync(entity).ConfigureAwait(false);

                TempData["success"] = "Certificate Request created successfully!";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                await LoadCombos().ConfigureAwait(false);

                TempData["error"] = ex.Message;

                _logger.LogError(ex, "Error creating certificate request.");

                return Page();
            }
        }
    }
}