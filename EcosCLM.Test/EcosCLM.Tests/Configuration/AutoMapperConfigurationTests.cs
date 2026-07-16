using AutoMapper;
using EcosCLM.Application.Infrastructure.Mappers;
using Xunit;

namespace EcosCLM.Tests.Configuration
{
    public class AutoMapperConfigurationTests
    {
        [Fact]
        public void AutoMapper_Configuration_Should_Be_Valid()
        {
            // 1. Carrega o IMapper utilizando a infraestrutura oficial da sua aplicação
            var mapper = MappingProfiles.LoadConfigurations();

            // 2. Recupera a configuração interna do mapper (ConfigurationProvider)
            var configuration = mapper.ConfigurationProvider;

            // 3. Valida se todas as regras e perfis bidirecionais estão 100% corretos
            configuration.AssertConfigurationIsValid();
        }
    }
}