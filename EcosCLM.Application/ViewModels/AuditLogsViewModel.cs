using AutoMapper;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.ViewModels
{
    public class AuditLogsViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public Guid IdCustumer { get; set; }
        public string LogType { get; set; }
        public string Log { get; set; }
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public string Hash { get; set; }

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
