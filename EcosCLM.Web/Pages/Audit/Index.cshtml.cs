using ClosedXML.Excel;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Security.Claims;
using TwoFactorAuthNet;

namespace EcosCLM.Web.Pages.Audit
{
    public class IndexModel : BasePageModel<AuditLogsViewModel>
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IAuditLogsRepository _repository;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public AuditLogsViewModel Search { get; set; }

        private readonly TwoFactorAuth _tfa;
        public string Secret { get; set; }
        public string Email { get; set; }

        [BindProperty(Name = "VerificationCode")]
        public string VerificationCode { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration config,
            IAuditLogsRepository repository,
            IEcosLoginService ecosLoginService,
            IHttpContextAccessor httpContextAccessor)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _repository = repository;

            Email = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ??
                    httpContextAccessor.HttpContext.User.FindFirst("email")?.Value;
        }

        public async Task<IActionResult> OnGetAsync(int p = 1)
        {
            PageCurrent = p;
            OnPostClear(false);

            Search = GetFilters();

            await GetDataAsync();
            AddGridConfig();
            return Page();
        }

        private void AddGridConfig()
        {
            GridConfig = new GridConfiguration
            {
                Title = "Audit Logs",
                AddPageUrl = "Add",
                SearchPlaceholder = null,
                SearchQuery = string.Empty,
                CurrentPage = Pager?.CurrentPage ?? 1,
                TotalPages = Pager?.TotalPages ?? 1,
                Headers = new List<string> { "Name", "Email", "Profile Type", "2FA" },
                ShowAddButton = false
            };
        }

        private async Task GetDataAsync()
        {
            try
            {
                var list = await _repository.GetAllWithPageAsync(PageSize, (PageCurrent - 1) * PageSize, Filter, OrderBy, OrderDirection, CustumerId);

                var totalItems = Pager?.TotalItems ?? list.Count; // Ajuste baseado em como sua paginação calcula o total geral

                Pager = new Pager(totalItems, PageCurrent, PageSize, MaxPages);
                Itens = list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GetData on dashboard page.");
                TempData["error"] = "An error occurred while loading dashboard data.";
            }
        }

        public async Task<IActionResult> OnPostDownloadAsync()
        {
            if (string.IsNullOrEmpty(Secret))
            {
                try
                {
                    var reportStream = await GenerateAuditReportAsync();
                    var zipStream = AddStreamToZip(reportStream, "AuditLogs.xlsx");

                    return File(zipStream.ToArray(), "application/zip", "AuditLogs.zip");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao gerar o relatório de auditoria.");
                    return RedirectToPage();
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostVerifyAndDownloadAsync()
        {
            if (_tfa.VerifyCode(Secret, VerificationCode))
            {
                try
                {
                    var reportStream = await GenerateAuditReportAsync();
                    var zipStream = AddStreamToZip(reportStream, "AuditLogs.xlsx");

                    return File(zipStream.ToArray(), "application/zip", "AuditLogs.zip");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao gerar o relatório de auditoria.");
                    return RedirectToPage();
                }
            }

            TempData["error"] = "Invalid verification code.";
            await GetDataAsync();
            return Page();
        }

        private async Task<MemoryStream> GenerateAuditReportAsync()
        {
            await _repository.CreateAsync(new AuditLogs
            {
                Date = DateTime.Now,
                User = Email,
                IdCustumer = CustumerId,
                Log = $"User: {Email} Generated a new audit logs Report!",
                LogType = "Audit"
            });

            var logsQuery = _repository.GetAll().Where(x => x.IdCustumer == CustumerId);

            if (Search.SearchStartDate.HasValue)
                logsQuery = logsQuery.Where(log => log.Date >= Search.SearchStartDate.Value);

            if (Search.SearchEndDate.HasValue)
                logsQuery = logsQuery.Where(log => log.Date <= Search.SearchEndDate.Value);

            if (!string.IsNullOrEmpty(Search.User))
                logsQuery = logsQuery.Where(log => log.User.Contains(Search.User));

            var logs = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(logsQuery);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Audit Logs");

            var headerRange = worksheet.Range("A1:F1");
            headerRange.Style.Font.Bold = true;

            worksheet.Cell(1, 2).Value = "Data";
            worksheet.Cell(1, 3).Value = "Usuário";
            worksheet.Cell(1, 5).Value = "Tipo de Log";
            worksheet.Cell(1, 6).Value = "Detalhes";

            int row = 2;
            foreach (var log in logs)
            {
                worksheet.Cell(row, 2).Value = log.Date.ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Cell(row, 3).Value = log.User;
                worksheet.Cell(row, 5).Value = log.LogType;
                worksheet.Cell(row, 6).Value = log.Log;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }

        private MemoryStream AddStreamToZip(MemoryStream fileStream, string fileName)
        {
            var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                fileStream.CopyTo(entryStream);
            }

            zipStream.Position = 0;
            return zipStream;
        }
    }
}