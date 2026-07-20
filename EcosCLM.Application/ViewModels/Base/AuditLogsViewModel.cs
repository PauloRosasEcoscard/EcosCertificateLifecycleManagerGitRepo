using AutoMapper;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.ViewModels
{
    public class AuditLogsViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; } = string.Empty;
        public Guid IdCustumer { get; set; }
        public string LogType { get; set; } = string.Empty;
        public string Log { get; set; } = string.Empty;
        public string SourceIp { get; set; } = string.Empty;
        public string DestinationIp { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;

        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
    }

    public class AccessLogsProfile : Profile
    {
        public AccessLogsProfile()
        {
            CreateMap<AuditLogs, AuditLogsViewModel>().ReverseMap();
        }
    }
}