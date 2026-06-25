using EcosCLM.Web.EcosLoginIntegration.Helper;

namespace EcosCLM.Web.EcosLoginIntegration.Extensions
{
    public static class BaseExtension
    {
        public static bool ToBoolean(this int value)
        {
            return value == 1;
        }

        public static bool AnyOrNull<T>(this IEnumerable<T> values)
        {
            if (values != null)
                return values.Any();
            else
                return false;
        }


        #region configuration
        public static string GetEncryptKeyFromConfig(this IConfiguration config)
        {
            var section = "EncryptKey";
            var key = config.GetValueFromConfig(section);

            if (!string.IsNullOrEmpty(key))
                return key;

            return ConstantHelper.EncryptKey;
        }

        public static string GetValueFromConfig(this IConfiguration config, string section)
        {
            if (config.ExistValueInConfig(section))
                return config.GetSection(section).Value;

            return string.Empty;
        }

        public static bool ExistValueInConfig(this IConfiguration config, string section)
        {
            if (string.IsNullOrEmpty(section))
                return false;

            return config.GetSection(section).Exists();
        }
        #endregion
    }
}
