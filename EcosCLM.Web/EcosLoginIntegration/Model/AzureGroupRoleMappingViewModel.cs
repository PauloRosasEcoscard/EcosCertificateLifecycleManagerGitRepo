using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class AzureGroupRoleMappingViewModel
    {
        public int Id { get; set; }
        public int AuthConfigAzureId { get; set; }
        [StringLength(255)]
        public string ExternalId { get; set; }
        [StringLength(255)]
        public string? DisplayName { get; set; }
        public Guid? PolicySystemProfileId { get; set; }
        public int? InternalProfileType { get; set; }
        [NotMapped]
        public string? ProfileValue { get; set; }
    }
}
