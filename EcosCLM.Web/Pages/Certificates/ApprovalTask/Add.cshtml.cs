using EcosCLM.Application.Extensions.Certificates;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Web.Pages.Certificates.ApprovalTask
{
    public class AddModel : BasePageModel<ApprovalTaskViewModel>
    {
        private readonly IApprovalTaskRepository _repository;
        private readonly ICertificateRequestRepository _certificateRequestRepository;
        private readonly IEcosLoginService _ecosLoginService;
        private readonly ILogger<AddModel> _logger;


        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IApprovalTaskRepository repository,
            ICertificateRequestRepository certificateRequestRepository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
            _certificateRequestRepository = certificateRequestRepository;
            _ecosLoginService = ecosLoginService;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            Item = new ApprovalTaskViewModel
            {
                StepOrder = 1,
                Status = "PENDING"
            };

            await LoadDropdowns().ConfigureAwait(false);

            return Page();
        }


        private async Task LoadDropdowns()
        {
            try
            {
                var requests = await _certificateRequestRepository
                    .GetAll()
                    .Where(x => x.CustomerId == CustumerId)
                    .ToListAsync()
                    .ConfigureAwait(false);


                ViewData["CertificateRequests"] =
                    requests.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.SubjectDn
                    })
                    .ToList();



                var usersResult = await _ecosLoginService
                    .GetPolicySystemCompanyUsers(CustumerId)
                    .ConfigureAwait(false);


                if (usersResult.IsSuccessful && usersResult.Data != null)
                {
                    ViewData["Users"] =
                        usersResult.Data.Select(x => new SelectListItem
                        {
                            Value = x.IdUser.ToString(),
                            Text = x.TxName
                        })
                        .ToList();
                }
                else
                {
                    ViewData["Users"] = new List<SelectListItem>();
                }



                var profilesResult = await _ecosLoginService
                    .GetAllProfilesList()
                    .ConfigureAwait(false);


                if (profilesResult.IsSuccessful && profilesResult.Data != null)
                {
                    ViewData["Profiles"] =
                        profilesResult.Data.Select(x => new SelectListItem
                        {
                            Value = x.IdProfile.ToString(),
                            Text = x.TxTitle
                        })
                        .ToList();
                }
                else
                {
                    ViewData["Profiles"] = new List<SelectListItem>();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading approval task dropdowns."
                );

                ViewData["CertificateRequests"] = new List<SelectListItem>();
                ViewData["Users"] = new List<SelectListItem>();
                ViewData["Profiles"] = new List<SelectListItem>();
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns().ConfigureAwait(false);

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


                TempData["success"] =
                    "Approval Task created successfully!";


                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                await LoadDropdowns().ConfigureAwait(false);

                TempData["error"] = ex.Message;

                _logger.LogError(
                    ex,
                    "Error creating approval task."
                );

                return Page();
            }
        }
    }
}