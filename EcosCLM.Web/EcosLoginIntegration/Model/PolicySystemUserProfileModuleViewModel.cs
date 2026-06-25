using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemUserProfileModuleViewModel
    {
        public Guid IdUserProfileModule { get; set; }
        public Guid IdUserProfile { get; set; }
        public Guid IdModule { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpd { get; set; }
        public bool IsDel { get; set; }
        public DateTime DtCreated { get; set; }

        public virtual PolicySystemModuleViewModel Module { get; set; }
        public virtual PolicySystemUserProfileViewModel UserProfile { get; set; }
    }

}