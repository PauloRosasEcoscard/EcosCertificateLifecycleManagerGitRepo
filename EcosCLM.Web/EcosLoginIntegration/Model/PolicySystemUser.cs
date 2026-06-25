using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemUser
    {

        [Key]
        public Guid IdUser { get; set; } = Guid.NewGuid();
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
    }

    public enum TypeProfile
    {
        Custom = 0, //Usuario personalizado
        Admin = 1, //Adminstrador com todas as permissões
        Audit = 2  //Auditor com todas as permissões de VISUALIZAÇÃO
    }
}
