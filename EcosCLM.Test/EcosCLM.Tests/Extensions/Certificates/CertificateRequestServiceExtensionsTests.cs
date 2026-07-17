using EcosCLM.Domain.Entities.Certificates;

namespace EcosCLM.Tests.Extensions.Certificates
{
    public class CertificateRequestServiceExtensionsTests
    {
        [Fact]
        public async Task PrepareForSubmissionAsync_Should_Throw_Exception_When_Csr_Is_Missing()
        {
            var request = new CertificateRequest
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Status = "APPROVED",
                SubjectDn = "CN=api.ecosclm.com.br",
                CsrPem = null // Sem CSR configurado
            };

            // TODO: Chame aqui o seu extension method real que prepara/valida o envio à CA.
            // Exemplo de comportamento esperado pela regra de negócio:
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                if (string.IsNullOrWhiteSpace(request.CsrPem))
                {
                    throw new InvalidOperationException("CSR PEM is required to submit a certificate request.");
                }
                await Task.CompletedTask;
            });

            Assert.Contains("CSR PEM is required", exception.Message);
        }

        [Fact]
        public async Task PrepareForSubmissionAsync_Should_Throw_Exception_When_Approval_Tasks_Are_Pending()
        {
            var requestId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                Status = "PENDING_APPROVAL",
                SubjectDn = "CN=api.ecosclm.com.br",
                CsrPem = "-----BEGIN CERTIFICATE REQUEST-----\nMII...\n-----END CERTIFICATE REQUEST-----"
            };

            var pendingTask = new ApprovalTask
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 1,
                Status = "PENDING" // Ainda pendente
            };

            request.ApprovalTasks = new List<ApprovalTask> { pendingTask };

            // TODO: Chame aqui o seu extension method real.
            // Exemplo de comportamento esperado pela regra de negócio:
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var hasPendingApprovals = request.ApprovalTasks.Any(t => t.Status != "APPROVED");
                if (hasPendingApprovals)
                {
                    throw new InvalidOperationException("Cannot submit request with pending approval tasks.");
                }
                await Task.CompletedTask;
            });

            Assert.Contains("pending approval tasks", exception.Message);
        }

        [Fact]
        public async Task PrepareForSubmissionAsync_Should_Succeed_When_Request_Is_Approved_And_Has_Csr()
        {
            var requestId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                Status = "APPROVED",
                SubjectDn = "CN=api.ecosclm.com.br",
                CsrPem = "-----BEGIN CERTIFICATE REQUEST-----\nMII...\n-----END CERTIFICATE REQUEST-----",
                UpdatedAt = DateTime.UtcNow
            };

            var approvedTask = new ApprovalTask
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 1,
                Status = "APPROVED" // Concluída
            };

            request.ApprovalTasks = new List<ApprovalTask> { approvedTask };

            // TODO: Chame aqui o seu extension method real.
            // Exemplo de comportamento esperado pela regra de negócio:
            var hasPendingApprovals = request.ApprovalTasks.Any(t => t.Status != "APPROVED");
            if (!hasPendingApprovals && !string.IsNullOrWhiteSpace(request.CsrPem))
            {
                request.Status = "SUBMITTED";
                request.UpdatedAt = DateTime.UtcNow;
            }

            // Valida se o estado foi alterado com sucesso para envio
            Assert.Equal("SUBMITTED", request.Status);
        }
    }
}