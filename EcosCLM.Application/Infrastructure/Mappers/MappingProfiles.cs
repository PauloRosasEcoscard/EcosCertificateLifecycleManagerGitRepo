using AutoMapper;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;

namespace EcosCLM.Application.Infrastructure.Mappers
{
    public static class MappingProfiles
    {
        public static IMapper LoadConfigurations(string[] configs = null)
        {
            if (configs == null || configs.Length == 0)
            {
                configs = new string[] { "EcosCLM.Application" };
            }

            var assemblies = configs.Select(name => Assembly.Load(name)).ToArray();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assemblies);
            }, NullLoggerFactory.Instance);

            return configuration.CreateMapper();
        }
    }
}