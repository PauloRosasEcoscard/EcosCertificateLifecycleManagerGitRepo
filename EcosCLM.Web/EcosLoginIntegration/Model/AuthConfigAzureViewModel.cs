namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class AuthConfigAzureViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid IdCustomer { get; set; }
        public string ClientAuthenticationMethod { get; set; }
        public string DiscoveryUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
        public string EmailClaim { get; set; }
        public string Scopes { get; set; }
        public bool? SourceClaimsFromAccessToken { get; set; }
        public string? RoleClaim { get; set; }
        public string? CustomHeaders { get; set; }
        public string? CustomRoles { get; set; }
        public string? CustumerName { get; set; }
        public MappingProfile MappingProfileFor { get; set; }
        private string UrlForLoginAzureAd { get; set; }
    }

    public enum MappingProfile
    {
        NoMapping = 0,
        AzureGroups = 1,
        AzureRoles = 2
    }
}
