namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class ContextApiUserViewModel
    {
        public DateTime? DtExpired { get; set; }
        public Guid ApiId { get; set; }
        public string ApiTitle { get; set; }
        public string ApiUrl { get; set; }
        public string ApiCallback { get; set; }
    }
}
