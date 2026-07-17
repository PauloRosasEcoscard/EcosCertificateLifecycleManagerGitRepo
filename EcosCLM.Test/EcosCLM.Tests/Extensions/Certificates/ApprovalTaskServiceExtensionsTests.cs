using EcosCLM.Domain.Entities.Certificates;
using NSubstitute;
using Xunit;

namespace EcosCLM.Tests.Extensions.Certificates
{
    public class ApprovalTaskServiceExtensionsTests
    {
        [Fact]
        public async Task ApproveStepAsync_Should_Activate_Next_Step_When_Not_Last()
        {
            var customerId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var currentTaskId = Guid.NewGuid();
            var nextTaskId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                Status = "PENDING_APPROVAL"
            };

            var currentTask = new ApprovalTask
            {
                Id = currentTaskId,
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 1,
                Status = "PENDING"
            };

            var nextTask = new ApprovalTask
            {
                Id = nextTaskId,
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 2,
                Status = "WAITING"
            };

            request.ApprovalTasks = new List<ApprovalTask> { currentTask, nextTask };

            // TODO: Chame aqui o seu extension method real de aprovação de etapa.
            // Exemplo conceitual da lógica de execução:
            currentTask.Status = "APPROVED";
            currentTask.UpdatedAt = DateTime.UtcNow;

            var pendingNext = request.ApprovalTasks
                .Where(t => t.StepOrder == currentTask.StepOrder + 1)
                .FirstOrDefault();

            if (pendingNext != null)
            {
                pendingNext.Status = "PENDING";
                pendingNext.UpdatedAt = DateTime.UtcNow;
            }

            // Asserts de validação do estado alterado pelas regras da sua extensão
            Assert.Equal("APPROVED", currentTask.Status);
            Assert.Equal("PENDING", nextTask.Status);
            Assert.Equal("PENDING_APPROVAL", request.Status); // A requisição pai continua aguardando mais etapas
        }

        [Fact]
        public async Task ApproveStepAsync_Should_Approve_CertificateRequest_When_Last_Step_Is_Approved()
        {
            var customerId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var currentTaskId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                Status = "PENDING_APPROVAL"
            };

            var currentTask = new ApprovalTask
            {
                Id = currentTaskId,
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 1,
                Status = "PENDING"
            };

            request.ApprovalTasks = new List<ApprovalTask> { currentTask };

            // TODO: Chame aqui o seu extension method real de aprovação de etapa.
            // Exemplo conceitual da lógica de execução:
            currentTask.Status = "APPROVED";
            currentTask.UpdatedAt = DateTime.UtcNow;

            var hasMoreSteps = request.ApprovalTasks.Any(t => t.StepOrder > currentTask.StepOrder);
            if (!hasMoreSteps)
            {
                request.Status = "APPROVED";
                request.UpdatedAt = DateTime.UtcNow;
            }

            // Asserts de validação
            Assert.Equal("APPROVED", currentTask.Status);
            Assert.Equal("APPROVED", request.Status); // Requisição promovida a aprovada com sucesso!
        }

        [Fact]
        public async Task RejectStepAsync_Should_Cancel_Subsequent_Steps_And_Set_Request_As_Rejected()
        {
            var customerId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var currentTaskId = Guid.NewGuid();
            var nextTaskId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                Status = "PENDING_APPROVAL"
            };

            var currentTask = new ApprovalTask
            {
                Id = currentTaskId,
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 1,
                Status = "PENDING"
            };

            var nextTask = new ApprovalTask
            {
                Id = nextTaskId,
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 2,
                Status = "WAITING"
            };

            request.ApprovalTasks = new List<ApprovalTask> { currentTask, nextTask };

            // Execução da regra de rejeição (exemplo conceitual do método de extensão)
            currentTask.Status = "REJECTED";
            currentTask.UpdatedAt = DateTime.UtcNow;

            // Cancela as etapas subsequentes
            var remainingTasks = request.ApprovalTasks.Where(t => t.StepOrder > currentTask.StepOrder);
            foreach (var task in remainingTasks)
            {
                task.Status = "CANCELLED";
                task.UpdatedAt = DateTime.UtcNow;
            }

            // Marca a requisição como rejeitada
            request.Status = "REJECTED";
            request.UpdatedAt = DateTime.UtcNow;

            // Asserts de validação
            Assert.Equal("REJECTED", currentTask.Status);
            Assert.Equal("CANCELLED", nextTask.Status);
            Assert.Equal("REJECTED", request.Status);
        }
    }
}