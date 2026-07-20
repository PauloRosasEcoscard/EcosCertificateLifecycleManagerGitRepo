using AutoMapper;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.ViewModels
{
    public class NotificationsViewModel
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class NotificationsProfile : Profile
    {
        public NotificationsProfile()
        {
            CreateMap<Notifications, NotificationsViewModel>().ReverseMap();
        }
    }
}