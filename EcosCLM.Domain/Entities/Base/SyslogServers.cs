using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosCLM.Domain.Entities.Base
{
    public class SyslogServers
    {
        public int Id { get; set; }
        public Guid CustumerId { get; set; }
        public bool? SyslogServerEnabled { get; set; }
        public bool? UseTls { get; set; }
        public string? VerificationCA { get; set; }
        public string? ServerAddress { get; set; }
        public string? Port { get; set; }
    }
}
