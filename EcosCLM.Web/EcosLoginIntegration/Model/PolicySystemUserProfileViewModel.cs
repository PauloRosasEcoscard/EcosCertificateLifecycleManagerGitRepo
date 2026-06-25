namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemUserProfileViewModel
    {
        public Guid IdUserProfile { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdProfile { get; set; }
        public DateTime DtCreated { get; set; }

        public virtual PolicySystemUserViewModel User { get; set; }
        public virtual PolicySystemProfileViewModel Profile { get; set; }
        public virtual PolicySystemUserProfileModuleViewModel[] UserProfileModules { get; set; }
    }

}