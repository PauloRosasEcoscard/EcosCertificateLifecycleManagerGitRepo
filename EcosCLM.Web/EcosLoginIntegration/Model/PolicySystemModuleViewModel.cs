using AutoMapper;
using System.ComponentModel.DataAnnotations;


namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemModuleViewModel
    {
        public Guid IdModule { get; set; }
        public string TxTitle { get; set; }
        public string TxController { get; set; }
        public string TxAction { get; set; }
        public string TxParams { get; set; }
        public bool IsInMenu { get; set; }
        public int NuStatus { get; set; }
        public DateTime DtCreated { get; set; }
    }
}