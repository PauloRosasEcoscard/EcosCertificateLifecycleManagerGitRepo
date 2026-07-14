using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Extensions.Base;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ecos_Cloud_Vhsm_Dashboard.Pages.Company.SyslogServers
{
    public class UpdateModel : BasePageModel<SyslogServersViewModel>
    {
        private readonly ISyslogServersRepository _repository;

        public UpdateModel(
            IConfiguration config,
            ISyslogServersRepository repository,
            IEcosLoginService ecosLoginService)
            : base(ecosLoginService, config)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                Item = await _repository.GetByIdAsync(id, CustumerId);
            }
            catch (NotFoundException)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _repository.EditAsync(Item);
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}