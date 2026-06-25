namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class ContextUserViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserTxPhone { get; set; }
        public int Profile { get; set; }
        public bool? IsAuth2fa { get; set; }
        public string Secret { get; set; }

        public string CustomerTitle { get; set; }
        public Guid IdCustomer { get; set; }
        public string CustomerRepresentative { get; set; }
        public string CustomerPhone { get; set; }
    }
}
