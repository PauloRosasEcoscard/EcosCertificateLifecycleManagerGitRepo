namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    // DTO para representar o usuário autenticado
    public class UserIntegrationDto
    {
        public Guid UserId { get; set; }
        public Guid IdCustomer { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int Profile { get; set; }
        public bool? IsAuth2fa { get; set; }
        public string Secret { get; set; }
    }

    // DTO para as Roles
    public class RoleIntegrationDto
    {
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpd { get; set; }
        public bool IsDel { get; set; }
        public string ModuleController { get; set; }
    }

    public class CustomerIntegrationDto
    {
        public Guid IdCustomer { get; set; }
        public string TxTitle { get; set; }
    }
}
