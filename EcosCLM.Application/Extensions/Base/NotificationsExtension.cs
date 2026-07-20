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
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(model);

            var entity = repository.ToEntity(model);
            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<NotificationsViewModel> CreateAsync(this INotificationsRepository repository, Notifications entity)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(entity);

            var query = await repository.AddAsync(entity).ConfigureAwait(false);
            return repository.ToViewModel(query);
        }

        public static async Task<List<NotificationsViewModel>> CreateMultipleAsync(this INotificationsRepository repository, List<string> Emails, Notifications notification)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(Emails);
            ArgumentNullException.ThrowIfNull(notification);

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

                var query = await repository.AddAsync(entity).ConfigureAwait(false);
                addedNotifications.Add(repository.ToViewModel(query));
            }

            return addedNotifications;
        }

        public static async Task<List<NotificationsViewModel>> CreateMultipleAsync(this INotificationsRepository repository, List<string> Emails, NotificationsViewModel notification)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(Emails);
            ArgumentNullException.ThrowIfNull(notification);

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

                var query = await repository.AddAsync(entity).ConfigureAwait(false);
                addedNotifications.Add(repository.ToViewModel(query));
            }

            return addedNotifications;
        }

        public static async Task<bool> DeleteOldNotificationsAsync(this INotificationsRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            var dateThreshold = DateTime.Now.AddDays(-30);

            var query = await repository.GetAll()
                .Where(x => x.Timestamp < dateThreshold)
                .ToListAsync()
                .ConfigureAwait(false);

            if (query.Count > 0)
            {
                await repository.DelManyAsync(query).ConfigureAwait(false);
            }

            return true;
        }
    }
}