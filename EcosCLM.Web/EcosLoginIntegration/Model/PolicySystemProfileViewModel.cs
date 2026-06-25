using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemProfileViewModel
    {
        public Guid IdProfile { get; set; }
        public string TxTitle { get; set; }
        public int NuStatus { get; set; }
        public DateTime DtCreated { get; set; }

        public virtual PolicySystemProfileModuleViewModel[] ProfileModules { get; set; }
    }

}