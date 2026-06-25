using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using EcosCLM.Web.Models;
using JW;
using Microsoft.AspNetCore.Mvc;

namespace Ecos_Cloud_Vhsm_Dashboard.Pages.Company.SyslogServers
{
    public class IndexModel : BasePageModel<SyslogServersViewModel>
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ISyslogServersRepository _repository;
        private readonly IAuditLogsRepository _auditLogs;
        private readonly ISyslogService _syslogService;

        public GridConfiguration GridConfig { get; set; } = new();

        [BindProperty(Name = "Search", SupportsGet = true)]
        public SyslogServersViewModel Search { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration config,
            ISyslogServersRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IAuditLogsRepository auditLogs,
            IEcosLoginService ecosLoginService,
            ISyslogService syslogService)
            : base(ecosLoginService, config)
        {
            _logger = logger;
            _repository = repository;
            _auditLogs = auditLogs;
            _syslogService = syslogService;
        }

        public IActionResult OnGet(int p = 1)
        {
            PageCurrent = p;
            OnPostClear(false);

            GetData();
            AddGridConfig();
            return Page();
        }

        private void AddGridConfig()
        {
            GridConfig = new GridConfiguration
            {
                // Define o título principal exibido no topo do painel de controle da listagem.
                Title = "Syslog Server",

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
                Headers = new List<string> { "Name", "Email", "Profile Type", "2FA" }
            };
        }

        public IActionResult OnPostSave()
        {
            Item.CustumerId = CustumerId;

            _repository.Create(Item);

            _auditLogs.Create(new AuditLogs
            {
                Date = DateTime.Now,
                User = Email,
                IdCustumer = CustumerId,
                Log = "User: " + Email + " created a new SyslogServer",
                LogType = "Company Management"
            }, _syslogService, HttpContextAccessor);

            return RedirectToPage("Index");
        }

        private void GetData()
        {
            int page = 0, offset = 0;
            var query = _repository.GetAllWithPage(CustumerId, page, offset, Filter);
            Pager = new Pager(query.Count(), PageCurrent, PageSize, MaxPages);
            Itens = query.Skip((Pager.CurrentPage - 1) * Pager.PageSize).Take(Pager.PageSize);
        }
    }
}
