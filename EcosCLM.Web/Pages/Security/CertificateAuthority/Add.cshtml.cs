using EcosCLM.Application.Extensions.Security;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Security;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Security.CertificateAuthority
{
    public class AddModel : BasePageModel<CertificateAuthorityViewModel>
    {
        private readonly ICertificateAuthorityRepository _repository;
        private readonly ILogger<AddModel> _logger;

        public AddModel(
            ILogger<AddModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            ICertificateAuthorityRepository repository)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            Item = new CertificateAuthorityViewModel();
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

                    TempData["success"] = "Certificate Authority created successfully!";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    _logger.LogError(ex, "Error creating certificate authority.");
                    return Page();
                }
            }

            return Page();
        }
    }
}