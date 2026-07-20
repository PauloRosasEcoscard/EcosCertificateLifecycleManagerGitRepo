using EcosCLM.Application.Interfaces;
using EcosCLM.Domain.Entities.Base;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace EcosCLM.Data.Services
{
    public class FileGenerator
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FileGenerator(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<GeneratedFile> GenerateFile(int delay, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using var scope = _scopeFactory.CreateScope();
            var _repository = scope.ServiceProvider.GetRequiredService<IAuditLogsRepository>();

            await Task.Delay(delay * 1000, ct).ConfigureAwait(false);
            var logs = _repository.GetAll().ToList();

            MemoryStream stream = new MemoryStream();

            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook!.Worksheets.Add("Planilha1");

                int lineAux = 0;

                foreach (var log in logs)
                {
                    lineAux++;
                    worksheet.Cells[lineAux, 1].Value = $"{log.User}";
                    worksheet.Cells[lineAux, 2].Value = $"{log.Log}";
                    worksheet.Cells[lineAux, 3].Value = $"{log.Date}";
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();

                package.Save();
            }

            stream.Position = 0;

            return new GeneratedFile
            {
                Content = stream.ToArray(),
                FileName = "relatorio.xlsx",
                ContentType = "application/octet-stream"
            };
        }
    }
}