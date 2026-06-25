using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Company.PolicySettings
{
    public class IndexModel : BasePageModel<PolicySettingsViewModel>
    {
        private readonly IAuditLogsRepository _auditLogs;
        private readonly IPolicySettingsRepository _PolicySettingsRepository;
        private readonly ISyslogService _syslogService;
        ILogger<IndexModel> _logger;
        IConfiguration _configuration;

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration config,
            IAuditLogsRepository auditLogs,
            IHttpContextAccessor httpContextAccessor,
            IPolicySettingsRepository PolicySettingsRepository,
            IEcosLoginService ecosLoginService,
            ISyslogService syslogService)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _auditLogs = auditLogs;
            _configuration = config;
            _PolicySettingsRepository = PolicySettingsRepository;

            _syslogService = syslogService;
        }

        public async Task<IActionResult> OnGet()
        {
            Item = _PolicySettingsRepository.GetByIdCustumer(CustumerId);

            if (Item == null)
            {

               Item = _PolicySettingsRepository.Create(new PolicySettingsViewModel
                        {
                            CustumerId = CustumerId,
                            TimeoutSession = 30
                        });

                RedirectToPage("Index");
            }

            if (Item.CustumerId != CustumerId)
            {
                TempData["warning"] = "Invalid operation";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (ModelState.IsValid)
            {
                _PolicySettingsRepository.Edit(Item);

                _auditLogs.Create(new AuditLogs
                {
                    Date = DateTime.Now,
                    User = Email,
                    IdCustumer = CustumerId,
                    Log = "User: " + Email + " updated an existing PolicySettings",
                    LogType = "Company Management"
                }, _syslogService, HttpContextAccessor);

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
