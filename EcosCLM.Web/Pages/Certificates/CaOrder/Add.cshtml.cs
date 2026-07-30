using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcosCLM.Web.Pages.Certificates.CaOrder
{
    public class AddModel : BasePageModel<CaOrderViewModel>
    {
        private readonly ICaOrderRepository _repository;
        private readonly ICertificateRequestRepository _certificateRequestRepository;
        private readonly ICertificateAuthorityRepository _certificateAuthorityRepository;
        private readonly ILogger<AddModel> _logger;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICaOrderRepository repository,
            ICertificateRequestRepository certificateRequestRepository,
            ICertificateAuthorityRepository certificateAuthorityRepository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
            _certificateRequestRepository = certificateRequestRepository;
            _certificateAuthorityRepository = certificateAuthorityRepository;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            Item = new CaOrderViewModel
            {
                Status = "CREATED"
            };

            await LoadCombos().ConfigureAwait(false);

            return Page();
        }


        private async Task LoadCombos()
        {
            try
            {
                var requests = await _certificateRequestRepository.GetAllWithPageAsync(
                    page: 1000,
                    offset: 0,
                    filter: null,
                    oderBy: "status",
                    customer: CustumerId
                ).ConfigureAwait(false);


                ViewData["CertificateRequests"] =
                    requests ?? new List<CertificateRequestViewModel>();


                var authorities = _certificateAuthorityRepository
                    .GetAll()
                    .Where(x => x.CustomerId == CustumerId)
                    .ToList();


                ViewData["CertificateAuthorities"] =
                    _certificateAuthorityRepository.ToListViewModel(authorities);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading CA order combos.");

                ViewData["CertificateRequests"] =
                    new List<CertificateRequestViewModel>();

                ViewData["CertificateAuthorities"] =
                    new List<CertificateAuthorityViewModel>();
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


                await _repository.CreateAsync(entity)
                    .ConfigureAwait(false);


                TempData["success"] = "CA Order created successfully!";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                await LoadCombos().ConfigureAwait(false);

                TempData["error"] = ex.Message;

                _logger.LogError(
                    ex,
                    "Error creating CA order."
                );

                return Page();
            }
        }
    }
}