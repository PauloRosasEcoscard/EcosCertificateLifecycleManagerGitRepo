using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcosCLM.Domain.Entities.Base
{
    public class AuditLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public Guid IdCustumer { get; set; }
        public string LogType { get; set; }
        public string Log { get; set; }
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public string Hash { get; set; }
    }
}
