using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Catalog;
using EcosCLM.Domain.Entities.Certificates;
using EcosCLM.Tests.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EcosCLM.Tests.Integration.Repositories.Certificates
{
    [Collection("DatabaseCollection")]
    public class CLMApplicationAndDomainRepositoryTests
    {
        private readonly DatabaseFixture _fixture;

        public CLMApplicationAndDomainRepositoryTests(DatabaseFixture fixture)
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
        public async Task Should_Persist_Application_And_Domain_Successfully()
        {
            using var context = CreateContext();
            var customerId = Guid.NewGuid();

            var app = new CLMApplication
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Code = "APP-TEST",
                Name = "Test Application",
                Criticality = "MEDIUM",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var domain = new ManagedDomain
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Fqdn = "test.ecosclm.com.br",
                ValidationMethod = "DNS",
                ValidationStatus = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.CLMApplications.Add(app);
            context.ManagedDomains.Add(domain);
            await context.SaveChangesAsync();

            var persistedApp = await context.CLMApplications.FindAsync(app.Id);
            var persistedDomain = await context.ManagedDomains.FindAsync(domain.Id);

            Assert.NotNull(persistedApp);
            Assert.Equal("APP-TEST", persistedApp.Code);
            Assert.NotNull(persistedDomain);
            Assert.Equal("test.ecosclm.com.br", persistedDomain.Fqdn);
        }

        [Fact]
        public async Task Should_Set_Null_On_Request_When_Application_Or_Domain_Is_Deleted()
        {
            using var context = CreateContext();
            var customerId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var domainId = Guid.NewGuid();

            var app = new CLMApplication
            {
                Id = appId,
                CustomerId = customerId,
                Code = "APP-DEL",
                Name = "App to Delete",
                Criticality = "LOW",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var domain = new ManagedDomain
            {
                Id = domainId,
                CustomerId = customerId,
                Fqdn = "del.ecosclm.com.br",
                ValidationMethod = "DNS",
                ValidationStatus = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var request = new CertificateRequest
            {
                Id = requestId,
                CustomerId = customerId,
                RequestType = "NEW",
                Status = "DRAFT",
                SubjectDn = "CN=del.ecosclm.com.br",
                CertificateRequestCLMApplicationId = appId,
                CertificateRequestDomainId = domainId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.CLMApplications.Add(app);
            context.ManagedDomains.Add(domain);
            context.CertificateRequests.Add(request);
            await context.SaveChangesAsync();

            context.CLMApplications.Remove(app);
            context.ManagedDomains.Remove(domain);
            await context.SaveChangesAsync();

            var persistedRequest = await context.CertificateRequests
                .FirstOrDefaultAsync(x => x.Id == requestId);

            Assert.NotNull(persistedRequest);
            Assert.Null(persistedRequest.CertificateRequestCLMApplicationId);
            Assert.Null(persistedRequest.CertificateRequestDomainId);
        }
    }
}