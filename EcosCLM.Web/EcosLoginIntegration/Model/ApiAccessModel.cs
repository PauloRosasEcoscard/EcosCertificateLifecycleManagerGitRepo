using Newtonsoft.Json;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class UserToken
    {
        public string token { get; set; }
    }

    public class User
    {
        public string UserID { get; set; }
        public string AccessKey { get; set; }
    }

    [JsonObject("tokenManagement")]
    public class TokenManagement
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("accessExpiration")]
        public int AccessExpiration { get; set; }

        [JsonProperty("refreshExpiration")]
        public int RefreshExpiration { get; set; }

    }
}
