using EcosCLM.Application.Interfaces;
using EcosCLM.Data.Services;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.Pages.Demos
{
    public class DownloadsModel : BasePageModel<AuditLogs>
    {

        private readonly IDownloadManager _downloadManager;
        private readonly FileGenerator _fileGenerator;
        [BindProperty]
        public int delay { get; set; }

        public DownloadsModel(
            IEcosLoginService ecosLoginService,
            IConfiguration config,
            FileGenerator fileGenerator,
            IDownloadManager downloadManager)
            : base(ecosLoginService, config)
        {
            _fileGenerator = fileGenerator;
            _downloadManager = downloadManager;
        }

        public async Task<IActionResult> OnPostNormalDownload()
        {
            var stream = await _fileGenerator.GenerateFile(delay, new CancellationToken()).ConfigureAwait(false);

            return File(stream.Content, stream.ContentType, stream.FileName);
        }

        public async Task<IActionResult> OnPostBackgroundDownload()
        {
            Console.WriteLine(">>> Processo iniciado");
            await _downloadManager.EnqueueAsync(Email, ct =>
                _fileGenerator.GenerateFile(delay, ct)).ConfigureAwait(false);

            return Page();
        }
    }
}
