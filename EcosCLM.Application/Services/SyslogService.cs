using EcosCLM.Application.Extensions;
using EcosCLM.Application.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace EcosCLM.Application.Services
{

    public class SyslogService : ISyslogService
    {
        private string _syslogServer;
        private int _syslogPort;
        private bool _syslogEnable;
        private readonly ISyslogServersRepository _syslogService;

        public SyslogService(ISyslogServersRepository syslogService)
        {
            _syslogService = syslogService;
        }

        public void Initialize(Guid IdCustumer)
        {
            var SysLogData = _syslogService.GetByIdCustumer(IdCustumer);
            _syslogServer = SysLogData.ServerAddress;
            _syslogPort = int.Parse(SysLogData.Port);
            _syslogEnable = SysLogData.SyslogServerEnabled;
        }

        public string SendLog(string appName, string message, SyslogSeverity severity = SyslogSeverity.Notice)
        {
            if (!_syslogEnable)
                return string.Empty;

            try
            {
                using (TcpClient client = new TcpClient(_syslogServer, _syslogPort))
                using (NetworkStream stream = client.GetStream())
                {
                    string logMessage = FormatSyslogMessage(appName, message, severity);
                    byte[] messageBytes = Encoding.UTF8.GetBytes(logMessage + "\n");

                    stream.Write(messageBytes, 0, messageBytes.Length);
                    stream.Flush();

                    return "Log enviado com sucesso!";
                }
            }
            catch (Exception ex)
            {
                return $"Erro ao enviar log: {ex.Message}";
            }
        }
        //$"Date: {auditLog.Date}, User: {auditLog.User}, IdCustomer: {auditLog.IdCustumer}, Log: {auditLog.Log}, LogType: {auditLog.LogType}"
        public string SendLog(string appName, object messege, SyslogSeverity severity = SyslogSeverity.Notice)
        {
            if (!_syslogEnable)
                return string.Empty;

            try
            {
                using (TcpClient client = new TcpClient(_syslogServer, _syslogPort))
                using (NetworkStream stream = client.GetStream())
                {
                    var formattedMessage = CreatedMessage(messege);

                    string logMessage = FormatSyslogMessage(appName, formattedMessage, severity);
                    byte[] messageBytes = Encoding.UTF8.GetBytes(logMessage + "\n");

                    stream.Write(messageBytes, 0, messageBytes.Length);
                    stream.Flush();

                    return "Log enviado com sucesso!";
                }
            }
            catch (Exception ex)
            {
                return $"Erro ao enviar log: {ex.Message}";
            }
        }

        private string CreatedMessage(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var properties = obj.GetType().GetProperties();
            var sb = new StringBuilder();

            foreach (var prop in properties)
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(obj, null);
                sb.AppendFormat("{0}: {1}, ", propName, propValue);
            }

            // Remove a última vírgula e espaço
            if (sb.Length > 2)
                sb.Length -= 2;

            return sb.ToString();
        }


        public string TestConnection()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(_syslogServer, _syslogPort);
                    return "Conexão com o servidor Syslog bem-sucedida!";
                }
            }
            catch (Exception ex)
            {
                return $"Falha ao conectar ao servidor Syslog: {ex.Message}";
            }
        }

        private string FormatSyslogMessage(string appName, string message, SyslogSeverity severity)
        {
            int pri = (8 * (int)SyslogFacility.User) + (int)severity;
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string host = Environment.MachineName;

            return $"<{pri}>1 {timestamp} {host} {appName} - - - {message}";
        }
    }

    public enum SyslogSeverity
    {
        Emergency = 0,   // Sistema inoperável
        Alert = 1,       // Ação imediata necessária
        Critical = 2,    // Condição crítica
        Error = 3,       // Erro
        Warning = 4,     // Aviso
        Notice = 5,      // Notificação normal
        Information = 6, // Informação
        Debug = 7        // Mensagens de depuração
    }

    public enum SyslogFacility
    {
        Kernel = 0,
        User = 1,
        Mail = 2,
        System = 3,
        Security = 4
    }
}
