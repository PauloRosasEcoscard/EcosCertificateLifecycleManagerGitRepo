using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemProfileModuleViewModel
    {
        public Guid IdProfileModule { get; set; }
        public Guid IdProfile { get; set; }
        public Guid IdModule { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpd { get; set; }
        public bool IsDel { get; set; }

        public virtual PolicySystemProfileViewModel Profile { get; set; }
        public virtual PolicySystemModuleViewModel Module { get; set; }
    }
}