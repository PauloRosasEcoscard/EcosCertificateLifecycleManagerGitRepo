namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class ContextRolesViewModel
    {
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpd { get; set; }
        public bool IsDel { get; set; }

        public string ProfileTitle { get; set; }
        public string ModuleTitle { get; set; }
        public string ModuleController { get; set; }
        public string ModuleAction { get; set; }
        public string ModuleParams { get; set; }
        public bool ModuleIsInMenu { get; set; }
    }
}
