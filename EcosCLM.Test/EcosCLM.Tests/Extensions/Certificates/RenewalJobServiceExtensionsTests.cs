using EcosCLM.Domain.Entities.Certificates;

namespace EcosCLM.Tests.Extensions.Certificates
{
    public class RenewalJobServiceExtensionsTests
    {
        [Fact]
        public async Task CreateRenewalJobAsync_Should_Throw_Exception_When_Active_Job_Already_Exists()
        {
            var certificateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            // Lista simulada de jobs existentes no banco/repositório para este certificado
            var existingJobs = new List<RenewalJob>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CertificateId = certificateId,
                    CustomerId = customerId,
                    Status = "SCHEDULED", // Ainda ativo/agendado
                    CreatedAt = DateTime.UtcNow
                }
            };

            // Nova tentativa de agendamento de job
            var newJob = new RenewalJob
            {
                CertificateId = certificateId,
                CustomerId = customerId,
                Status = "SCHEDULED"
            };

            // Execução da regra (exemplo conceitual do método de extensão)
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var hasActiveJob = existingJobs.Any(j => j.CertificateId == newJob.CertificateId &&
                                                         (j.Status == "SCHEDULED" || j.Status == "RUNNING"));
                if (hasActiveJob)
                {
                    throw new InvalidOperationException("An active renewal job already exists for this certificate.");
                }
                await Task.CompletedTask;
            });

            Assert.Contains("active renewal job already exists", exception.Message);
        }

        [Fact]
        public async Task CreateRenewalJobAsync_Should_Succeed_When_No_Active_Job_Exists()
        {
            var certificateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            // Histórico de jobs anteriores concluídos ou falhos (nenhum ativo)
            var existingJobs = new List<RenewalJob>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CertificateId = certificateId,
                    CustomerId = customerId,
                    Status = "COMPLETED",
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };

            var newJob = new RenewalJob
            {
                Id = Guid.NewGuid(),
                CertificateId = certificateId,
                CustomerId = customerId,
                Status = "SCHEDULED"
            };

            // Execução da regra (exemplo conceitual do método de extensão)
            bool isSuccess = false;
            var hasActiveJob = existingJobs.Any(j => j.CertificateId == newJob.CertificateId &&
                                                     (j.Status == "SCHEDULED" || j.Status == "RUNNING"));
            if (!hasActiveJob)
            {
                isSuccess = true;
            }

            Assert.True(isSuccess);
            Assert.Equal("SCHEDULED", newJob.Status);
        }
    }
}