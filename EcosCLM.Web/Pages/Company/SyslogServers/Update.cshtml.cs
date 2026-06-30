using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Extensions;
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

        public IActionResult OnGet(int id)
        {
            try
            {
                Item = _repository.GetById(id, CustumerId);
            }
            catch (NotFoundException)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _repository.Edit(Item);
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}