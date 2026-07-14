using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Data.Repositories
{
    public class NotificationsRepository : BaseRepository<Notifications, NotificationsViewModel>, INotificationsRepository
    {
        public NotificationsRepository(EcosCLMContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }
    }
}
