using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ecos_Cloud_Vhsm_Dashboard.Pages.Company.SyslogServers
{
    public class AddModel : BasePageModel<SyslogServersViewModel>
    {
        private readonly ISyslogServersRepository _repository;
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ISyslogService _syslogService;

        public AddModel(
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

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                Item.CustumerId = CustumerId;

                // Passo 1: Converter o ViewModel para a Entidade
                // Isso utiliza o método ToEntity do repositório/BaseRepository para mapeamento
                var entity = _repository.ToEntity(Item);

                // Passo 2: Cria o registro de Auditoria (antes de salvar)
                _auditLogs.Create(new AuditLogs
                {
                    Date = DateTime.Now,
                    User = Email,
                    IdCustumer = CustumerId,
                    Log = "User: " + Email + " created a new SyslogServer.",
                    LogType = "Company Management"
                }, _syslogService, HttpContextAccessor);

                // Passo 3: Adiciona a Entidade (SyslogServers) no repositório
                // O método Add agora recebe o tipo correto: Entidade (SyslogServers)
                _repository.Add(entity);

                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}