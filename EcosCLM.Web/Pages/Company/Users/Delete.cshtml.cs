using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Company.Users
{
    public class DeleteModel : BasePageModel<PolicySystemUserViewModel>
    {
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ILogger<DeleteModel> _logger;
        private readonly ISyslogService _syslogService;
        private readonly IEcosLoginService _ecosLoginService;

        public bool CanDelete { get; set; } = true;

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IEcosLoginService ecosLoginService,
            IAuditLogsRepository auditLogs,
            ISyslogService syslogService)
            : base(ecosLoginService, configuration)
        {
            _logger = logger;
            _auditLogs = auditLogs;
            _syslogService = syslogService;
            _ecosLoginService = ecosLoginService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            await GetData(id);

            if (Item == null)
                return RedirectToPage("Index");

            if (Item.TxEmail == Email)
            {
                CanDelete = false;
                TempData["ErrorMessage"] = "you can't delete yourself";
            }

            if (Item.IdCustomer != CustumerId)
            {
                TempData["warning"] = "Invalid operation";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var result = await _ecosLoginService.DeletePolicySystemUser(Item.IdUser);

                if (result.IsSuccessful)
                {
                    _auditLogs.Create(new AuditLogs
                    {
                        Date = DateTime.Now,
                        User = Email,
                        IdCustumer = CustumerId,
                        Log = $"User: {Email} deleted a user",
                        LogType = "Company Management"
                    }, _syslogService, HttpContextAccessor);
                }
                else
                {
                    TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error executing user deletion.");
                return Page();
            }

            return RedirectToPage("Index");
        }

        private async Task GetData(Guid id)
        {
            try
            {
                var result = await _ecosLoginService.GetPolicySystemUserById(id);

                if (result.IsSuccessful && result.Data != null)
                {
                    Item = result.Data;
                }
                else
                {
                    TempData["warning"] = $"Status Code: {result.StatusCode} - {result.ErrorMessage}|";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "Error retrieving data for user ID: {UserId}", id);
            }
        }
    }
}