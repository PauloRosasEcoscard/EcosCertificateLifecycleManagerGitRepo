using AutoMapper;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Data.Services;
using EcosCLM.Domain.DataTypes;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EcosCLM.Web.Pages.Company.IdentityProvider
{
    public class DeleteModel : BasePageModel<AuthConfigAzureViewModel>
    {
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ILogger<DeleteModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISyslogService _syslogService;

        public Dictionary<Guid, string> Customers;

        public DeleteModel(
            ILogger<DeleteModel> logger,
            IConfiguration configuration,
            IConfiguration config,
            IAuditLogsRepository auditLogs,
            IHttpContextAccessor httpContextAccessor,
            IEcosLoginService ecosLoginService,
            ISyslogService syslogService)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _auditLogs = auditLogs;
            _configuration = configuration;
            _syslogService = syslogService;
        }


        public async Task<IActionResult> OnGet(int id)
        {
            await GetData(id);

            if (Item == null)
                return RedirectToPage("Index");


            if (Item.IdCustomer != CustumerId)
            {
                TempData["warning"] = "Invalid operation";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                string url = _configuration.GetSection("AppSettings:Clients:Login").Value;
                string uri = string.Format(PolicySystemUris.deleteIdentityProvider, Item.Id);

                HttpResponseMessage response = await HttpRequestService.DeleteAsync(string.Concat(url, uri), _logger);
                string responseContentHttp = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _auditLogs.Create(new AuditLogs
                    {
                        Date = DateTime.Now,
                        User = Email,
                        IdCustumer = CustumerId,
                        Log = "User: " + Email + " deleted an Identity Provider",
                        LogType = "Company Management"
                    }, _syslogService, HttpContextAccessor);
                }
                else
                {
                    TempData["warning"] = $"Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}.\n{responseContentHttp}|";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex.Message);
                return Page();
            }
            return RedirectToPage("Index");
        }

        async Task GetData(int id)
        {
            try
            {
                await GetItem(id);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex.Message);
            }
        }

        private async Task GetItem(int id)
        {
            string url = _configuration.GetSection("AppSettings:Clients:Login").Value;
            string uri = string.Format(PolicySystemUris.getIdentityProviderByCustumerIdentityProvider, CustumerId, id);

            HttpResponseMessage response = await HttpRequestService.GetAsync(string.Concat(url, uri), _logger);
            string responseContentHttp = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Item = JsonConvert.DeserializeObject<AuthConfigAzureViewModel>(responseContentHttp);
            }
            else
            {
                TempData["warning"] = $"Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}.\n{responseContentHttp}|";
            }
        }
    }
}
