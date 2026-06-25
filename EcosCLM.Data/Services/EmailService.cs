using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace EcosCLM.Data.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ApiSendAuditEmailAsync(string userEmail, string userIpAddress)
        {
            string url = _configuration.GetSection("AppSettings:Clients:Login").Value;
            string uri = $"/auth/SendEmail";

            string emailBody = "<h1 style=\"color: #e63572; font-family: Arial;\">Ecos Cloud VHSM</h1>" +
                               $"<p style=\"font-family: Arial;\">Your account (<a href=\"mailto:{userEmail}\">{userEmail}</a>) was accessed from the following IP address ({userIpAddress}).</p>" +
                               "<p style=\"font-family: Arial;\">If you do not recognize this activity please change your password immediatly.</p>";

            var body = new
            {
                FromEmail = "atendimento.ativacao@rtm.net.br",
                ToEmail = userEmail,
                Subject = "Ecos Dashboard New Login",
                Body = emailBody
            };

            ILogger logger = null;
            HttpResponseMessage response = await HttpRequestService.PostAsync(string.Concat(url, uri), body, logger);
        }
    }
}
