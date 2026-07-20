using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ecos_Cloud_Vhsm_Dashboard.Pages.Company.SyslogServers
{
    public class AddModel : BasePageModel<SyslogServersViewModel>
    {
        private readonly ISyslogServersRepository _repository;

        public AddModel(
            IConfiguration config,
            ISyslogServersRepository repository,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, config)
        {
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            Item = new SyslogServersViewModel
            {
                CustumerId = CustumerId,
                SyslogServerEnabled = false,
                UseTls = false
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Item.CustumerId = CustumerId;

                var entity = _repository.ToEntity(Item);
                await _repository.AddAsync(entity).ConfigureAwait(false);

                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}