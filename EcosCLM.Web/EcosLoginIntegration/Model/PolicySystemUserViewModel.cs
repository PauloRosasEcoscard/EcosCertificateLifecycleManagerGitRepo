namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemUserViewModel
    {
        public Guid IdUser { get; set; }
        public Guid IdCustomer { get; set; }
        public string TxName { get; set; }
        public string TxEmail { get; set; }
        public string TxPhone { get; set; }
        public string TxPassword { get; set; }
        public string TxSalt { get; set; }
        public int NuStatus { get; set; }
        public int Profile { get; set; }
        public Guid? ResetToken { get; set; }
        public int FailedAccessAttempts { get; set; }
        public DateTime? LockoutEndDate { get; set; }
        public DateTime? TokenValidUntil { get; set; }
        public bool Auth2fa { get; set; }
        public string Secret { get; set; }
        public DateTime DtCreated { get; set; }
        public bool IsBlocked { get; set; }

        public string? IdProfile { get; set; }

        public virtual PolicySystemCustomerViewModel Customer { get; set; }
        public virtual ICollection<PolicySystemUserProfileViewModel> UserProfiles { get; set; }
    }
}
