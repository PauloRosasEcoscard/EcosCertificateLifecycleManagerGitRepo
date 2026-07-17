using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Certificates;
using EcosCLM.Tests.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EcosCLM.Tests.Integration.Repositories.Certificates
{
    [Collection("DatabaseCollection")]
    public class CertificateRequestRepositoryTests
    {
        private readonly DatabaseFixture _fixture;

        public CertificateRequestRepositoryTests(DatabaseFixture fixture)
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
        public async Task Should_Cascade_Delete_SanDns_When_Request_Is_Deleted()
        {
            using var context = CreateContext();
            var requestId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                SubjectDn = "CN=ecosclm.com.br",
                RequestType = "NEW",
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var sanDns = new CertificateRequestSanDns
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                RequestId = requestId,
                DnsName = "alt.ecosclm.com.br",
                CreatedAt = DateTime.UtcNow
            };

            context.CertificateRequests.Add(request);
            context.CertificateRequestSanDns.Add(sanDns);
            await context.SaveChangesAsync();

            context.CertificateRequests.Remove(request);
            await context.SaveChangesAsync();

            var persistedSan = await context.CertificateRequestSanDns
                .FirstOrDefaultAsync(x => x.RequestId == requestId);

            Assert.Null(persistedSan);
        }
    }
}