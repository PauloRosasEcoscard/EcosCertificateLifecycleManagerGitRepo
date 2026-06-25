namespace EcosCLM.Domain.Entities.Base
{
    public class PolicySettings
    {
        public Guid Id { get; set; }
        public Guid CustumerId { get; set; }
        public int TimeoutSession { get; set; }
    }
}
