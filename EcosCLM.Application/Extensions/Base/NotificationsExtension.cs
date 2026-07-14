using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Application.Extensions.Base
{
    public static class NotificationsExtension
    {
        public static async Task<NotificationsViewModel> CreateAsync(this INotificationsRepository repository, NotificationsViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }

        public static async Task<NotificationsViewModel> CreateAsync(this INotificationsRepository repository, Notifications entity)
        {
            var query = await repository.AddAsync(entity);
            return repository.ToViewModel(query);
        }

        public static async Task<List<NotificationsViewModel>> CreateMultipleAsync(this INotificationsRepository repository, List<string> Emails, Notifications notification)
        {
            var addedNotifications = new List<NotificationsViewModel>();

            foreach (var email in Emails)
            {
                var entity = new Notifications
                {
                    Timestamp = notification.Timestamp,
                    User = email,
                    Message = notification.Message,
                    Link = notification.Link,
                    Icon = notification.Icon
                };

                var query = await repository.AddAsync(entity);
                addedNotifications.Add(repository.ToViewModel(query));
            }

            return addedNotifications;
        }

        public static async Task<List<NotificationsViewModel>> CreateMultipleAsync(this INotificationsRepository repository, List<string> Emails, NotificationsViewModel notification)
        {
            var addedNotifications = new List<NotificationsViewModel>();

            foreach (var email in Emails)
            {
                var entity = new Notifications
                {
                    Timestamp = notification.Timestamp,
                    User = email,
                    Message = notification.Message,
                    Link = notification.Link,
                    Icon = notification.Icon
                };

                var query = await repository.AddAsync(entity);
                addedNotifications.Add(repository.ToViewModel(query));
            }

            return addedNotifications;
        }

        public static async Task<bool> DeleteOldNotificationsAsync(this INotificationsRepository repository)
        {
            var dateThreshold = DateTime.Now.AddDays(-30);

            var query = await repository.GetAll()
                .Where(x => x.Timestamp < dateThreshold)
                .ToListAsync();

            if (query.Any())
            {
                await repository.DelManyAsync(query);
            }

            return true;
        }
    }
}