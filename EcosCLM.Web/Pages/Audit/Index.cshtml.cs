using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO.Compression;
using System.Security.Claims;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace EcosCLM.Web.Pages.Audit
{
    public class IndexModel : BasePageModel<AuditLogsViewModel>
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IAuditLogsRepository _repository;
        private readonly ISyslogService _syslogService;
        private readonly IEcosLoginService _ecosLoginService;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search")]
        public AuditLogsViewModel Search { get; set; }

        TwoFactorAuth tfa;
        public string Secret { get; set; }
        public string Email { get; set; }

        [BindProperty(Name = "VerificationCode")]
        public string VerificationCode { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration config,
            IAuditLogsRepository repository,
            ISyslogService syslogService,
            IEcosLoginService ecosLoginService,
            IHttpContextAccessor httpContextAccessor)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _repository = repository;
            _syslogService = syslogService;
            _ecosLoginService = ecosLoginService;

            Email = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ??
                           httpContextAccessor.HttpContext.User.FindFirst("email")?.Value;

        }
        public IActionResult OnGet(int p = 1)
        {
            PageCurrent = p;
            OnPostClear(false);

            Search = GetFilters();

            GetData();
            AddGridConfig();
            return Page();
        }

        private void AddGridConfig()
        {
            GridConfig = new GridConfiguration
            {
                // Define o título principal exibido no topo do painel de controle da listagem.
                Title = "Audit Logs",

                // URL para a página de criação. Se for nula ou vazia, o botão "New" desaparece automaticamente.
                AddPageUrl = "Add",

                // Texto do placeholder do campo de busca por texto padrão. 
                // ATENÇÃO: Se for deixado nulo ou vazio, o input 'Search.TxName' NÃO será renderizado na tela.
                SearchPlaceholder = null,

                // Mantém o valor digitado no input de busca padrão preenchido após o postback/filtragem.
                SearchQuery = string.Empty,

                // Informa à grid o índice da página atual que está sendo renderizada (essencial para a paginação).
                CurrentPage = Pager?.CurrentPage ?? 1,

                // Quantidade total de páginas calculadas no servidor. Se for maior ou igual a 1, renderiza o paginador no rodapé.
                TotalPages = Pager?.TotalPages ?? 1,

                // Lista de cabeçalhos das colunas (útil para referências internas ou logs, mantido para compatibilidade).
                Headers = new List<string> { "Name", "Email", "Profile Type", "2FA" },

                ShowAddButton = false
            };
        }

        private async Task GetData()
        {
            try
            {
                var query = _repository.GetAllWithPage(0, 0, Filter, OrderBy, OrderDirection, CustumerId);
                int totalItems = query.Count();

                Pager = new Pager(totalItems, PageCurrent, PageSize, MaxPages);

                Itens = query.Skip((Pager.CurrentPage - 1) * Pager.PageSize).Take(Pager.PageSize).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GetData on dashboard page.");
                TempData["error"] = "An error occurred while loading dashboard data.";
            }
        }

        public IActionResult OnPostDownload()
        {
            if (string.IsNullOrEmpty(Secret))
            {
                try
                {
                    var reportStream = GenerateAuditReport(); // Gera o relatório em Excel
                    var zipStream = AddStreamToZip(reportStream, "AuditLogs.xlsx"); // Compacta o arquivo

                    return File(zipStream.ToArray(), "application/zip", "AuditLogs.zip");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao gerar o relatório de auditoria.");
                    return RedirectToPage();
                }
            }
            else
            {
                return Page();
            }
        }


        public IActionResult OnPostVerifyAndDownload()
        {
            if (tfa.VerifyCode(Secret, VerificationCode) == true)
            {
                try
                {
                    var reportStream = GenerateAuditReport(); // Gera o relatório em Excel
                    var zipStream = AddStreamToZip(reportStream, "AuditLogs.xlsx"); // Compacta o arquivo

                    return File(zipStream.ToArray(), "application/zip", "AuditLogs.zip");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao gerar o relatório de auditoria.");
                    return RedirectToPage();
                }
            }
            else
            {
                TempData["error"] = "Invalid verification code.";
                GetData();
                return Page();
            }
        }

        private MemoryStream GenerateAuditReport()
        {
            _repository.Create(new AuditLogs
            {
                Date = DateTime.Now,
                User = Email,
                IdCustumer = CustumerId,
                Log = $"User: {Email} Genereted a new audit logs Report!",
                LogType = "Audit"
            }, _syslogService, HttpContextAccessor);

            var logsQuery = _repository.GetAll().Where(x => x.IdCustumer == CustumerId);

            // Aplica os filtros de data, se definidos
            if (Search.SearchStartDate.HasValue)
                logsQuery = logsQuery.Where(log => log.Date >= Search.SearchStartDate.Value);

            if (Search.SearchEndDate.HasValue)
                logsQuery = logsQuery.Where(log => log.Date <= Search.SearchEndDate.Value);

            // Aplica o filtro de usuário, se especificado
            if (!string.IsNullOrEmpty(Search.User))
                logsQuery = logsQuery.Where(log => log.User.Contains(Search.User));

            var logs = logsQuery.ToList();

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
