using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace EcosCLM.Application.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IPolicySettingsRepository _policySettingsRepository;
        private readonly IMemoryCache _cache;
        private const string CacheKeyBase = "SessionTimeout";

        public ConfigurationService(IPolicySettingsRepository policySettingsRepository, IMemoryCache cache)
        {
            _policySettingsRepository = policySettingsRepository;
            _cache = cache;
        }

        public async Task<int> GetSessionTimeoutMinutesAsync(string customerName)
        {
            var cacheKey = $"{CacheKeyBase}_{customerName}";
            PolicySettingsViewModel policySettings = null;

            if (_cache.TryGetValue(cacheKey, out int timeout))
            {
                return timeout;
            }

            if (Guid.TryParse(customerName, out Guid customerGuid))
            {
                policySettings = _policySettingsRepository.GetByIdCustumer(customerGuid);
            }

            if (policySettings != null)
            {
                var timeoutMinutes = policySettings.TimeoutSession;
                _cache.Set(cacheKey, timeoutMinutes, TimeSpan.FromMinutes(10));
                return timeoutMinutes;
            }

            // Return a default value if not found
            return 30;
        }
    }
}
