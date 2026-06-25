using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;


namespace Ecos_Cloud_Vhsm_Dashboard.Pages.Company.SyslogServers
{
    public class UpdateModel : BasePageModel<SyslogServersViewModel>
    {
        private readonly ISyslogServersRepository _repository;
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ISyslogService _syslogService;

        public UpdateModel(
            IConfiguration config,
            ISyslogServersRepository repository,
            IAuditLogsRepository auditLogs,
            IHttpContextAccessor httpContextAccessor,
            IEcosLoginService ecosLoginService,
            ISyslogService syslogService)
            : base(ecosLoginService, config)
        {
            _repository = repository;
            _auditLogs = auditLogs;
            _syslogService = syslogService;
        }

        public IActionResult OnGet(int id)
        {
            try
            {
                Item = _repository.GetById(id, CustumerId);
            }
            catch (NotFoundException ex)
            {
                RedirectToPage("Index");
            }

            return Page();
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _auditLogs.Create(new AuditLogs
                {
                    Date = DateTime.Now,
                    User = Email,
                    IdCustumer = CustumerId,
                    Log = "User: " + Email + " updated an existing SyslogServer",
                    LogType = "Company Management"
                }, _syslogService, HttpContextAccessor);

                _repository.Edit(Item);

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
