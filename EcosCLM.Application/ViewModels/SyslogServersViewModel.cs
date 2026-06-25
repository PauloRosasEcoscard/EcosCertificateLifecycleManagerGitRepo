using AutoMapper;
using EcosCLM.Domain.Entities;

namespace EcosCLM.Application.ViewModels
{
    public class SyslogServersViewModel
    {
        public int Id { get; set; }
        public Guid CustumerId { get; set; }
        public bool SyslogServerEnabled { get; set; }
        public bool UseTls { get; set; }
        public string? VerificationCA { get; set; }
        public string? ServerAddress { get; set; }
        public string? Port { get; set; }

    }

    public class SyslogServersProfile : Profile
    {
        public SyslogServersProfile()
        {
            CreateMap<SyslogServers, SyslogServersViewModel>().ReverseMap();
        }
    }
}
