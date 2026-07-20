using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using EcosCLM.Application.Extensions.Base;

namespace Ecos_Cloud_Vhsm_Dashboard.Pages.Company.SyslogServers
{
    public class DeleteModel : BasePageModel<SyslogServersViewModel>
    {
        private readonly ISyslogServersRepository _repository;

        public DeleteModel(
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
                Item = await _repository.GetByIdAsync(id, CustumerId).ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _repository.DeleteAsync(Item.Id).ConfigureAwait(false);
                return RedirectToPage("Index");
            }
            catch
            {
                return Page();
            }
        }
    }
}