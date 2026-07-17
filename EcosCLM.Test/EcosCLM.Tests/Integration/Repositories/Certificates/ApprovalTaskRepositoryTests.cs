using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Certificates;
using EcosCLM.Tests.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EcosCLM.Tests.Integration.Repositories.Certificates
{
    [Collection("DatabaseCollection")]
    public class ApprovalTaskRepositoryTests
    {
        private readonly DatabaseFixture _fixture;

        public ApprovalTaskRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        private EcosCLMContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<EcosCLMContext>()
                .UseSqlServer(_fixture.ConnectionString)
                .Options;

            return new EcosCLMContext(options);
        }

        [Fact]
        public async Task Should_Persist_ApprovalTasks_In_Sequential_Order()
        {
            using var context = CreateContext();
            var customerId = Guid.NewGuid();
            var requestId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                RequestType = "NEW",
                Status = "PENDING_APPROVAL",
                SubjectDn = "CN=api.ecosclm.com.br",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var firstApproval = new ApprovalTask
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 1,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var secondApproval = new ApprovalTask
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 2,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.CertificateRequests.Add(request);
            context.ApprovalTasks.Add(firstApproval);
            context.ApprovalTasks.Add(secondApproval);
            await context.SaveChangesAsync();

            // Busca as tarefas ordenadas por etapa para simular a fila de aprovação
            var sortedTasks = await context.ApprovalTasks
                .Where(x => x.RequestId == requestId)
                .OrderBy(x => x.StepOrder)
                .ToListAsync();

            Assert.Equal(2, sortedTasks.Count);
            Assert.Equal(1, sortedTasks[0].StepOrder);
            Assert.Equal(2, sortedTasks[1].StepOrder);
        }

        [Fact]
        public async Task Should_Update_ApprovalTask_Status_Successfully()
        {
            using var context = CreateContext();
            var customerId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                RequestType = "NEW",
                Status = "PENDING_APPROVAL",
                SubjectDn = "CN=auth.ecosclm.com.br",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var task = new ApprovalTask
            {
                Id = taskId,
                CustomerId = customerId,
                RequestId = requestId,
                StepOrder = 1,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.CertificateRequests.Add(request);
            context.ApprovalTasks.Add(task);
            await context.SaveChangesAsync();

            // Modifica o status da tarefa para simular a aprovação
            var persistedTask = await context.ApprovalTasks.FindAsync(taskId);
            Assert.NotNull(persistedTask);

            persistedTask.Status = "APPROVED";
            persistedTask.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            var updatedTask = await context.ApprovalTasks.FindAsync(taskId);
            Assert.NotNull(updatedTask);
            Assert.Equal("APPROVED", updatedTask.Status);
        }
    }
}