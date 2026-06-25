using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcosCLM.Domain.Entities.Base
{
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
    }
}
