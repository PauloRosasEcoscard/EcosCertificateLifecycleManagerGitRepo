using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Web.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Web.Pages
{
    public class DownloadModel : BasePageModel<AuditLogsViewModel>
    {
        readonly IDownloadJobsRepository _repository;

        public DownloadModel(
            IEcosLoginService ecosLoginService,
            IConfiguration config,
            IDownloadJobsRepository repository)
            : base(ecosLoginService, config)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            var job = await _repository.FindOneAsync(x => x.Id == id && x.User == Email);
            var referer = Request.Headers["Referer"].ToString();
            string location = (!string.IsNullOrEmpty(referer)) ? referer : "/Index";

            if (job == null) 
            {
                TempData["warning"] = "File Not found!";
                return RedirectToPage(location);
            }

            if (job.Status != DownloadStatus.Ready) 
            {
                if(job.Status == DownloadStatus.Error)
                {
                    TempData["error"] = "An error ocurred while generating your file, please try again!";
                }

                if(job.Status == DownloadStatus.Pending || job.Status == DownloadStatus.Processing)
                {
                    TempData["warning"] = "Your file is not ready yet!";
                }

                return RedirectToPage(location);
            }

            var stream = System.IO.File.OpenRead(job.FilePath);

            return File(stream, job.ContentType, job.FileName);
        }
    }
}
