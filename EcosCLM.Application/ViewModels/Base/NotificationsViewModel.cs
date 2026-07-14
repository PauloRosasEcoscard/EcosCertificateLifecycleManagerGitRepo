using AutoMapper;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.ViewModels
{
    public class NotificationsViewModel
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }

    }

    public class NotificationsProfile : Profile
    {
        public NotificationsProfile()
        {
            CreateMap<Notifications, NotificationsViewModel>().ReverseMap();
        }
    }
}
