using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities;

namespace EcosCLM.Application.Extensions
{
    public static class NotificationsExtension
    {

        public static NotificationsViewModel Create(this INotificationsRepository repository, NotificationsViewModel model)
        {
            var entity = repository.ToEntity(model);
            var query = repository.Add(entity);
            return repository.ToViewModel(query);
        }
        public static NotificationsViewModel Create(this INotificationsRepository repository, Notifications entity)
        {
            var query = repository.Add(entity);
            return repository.ToViewModel(query);
        }

        public static List<NotificationsViewModel> CreateMultiple(this INotificationsRepository repository, List<string> Emails, Notifications notification)
        {
            List<NotificationsViewModel> addedNotifications = new List<NotificationsViewModel>();

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

                var query = repository.Add(entity);

                addedNotifications.Add(repository.ToViewModel(query));
            }

            return addedNotifications;
        }

        public static List<NotificationsViewModel> CreateMultiple(this INotificationsRepository repository, List<string> Emails, NotificationsViewModel notification)
        {
            List<NotificationsViewModel> addedNotifications = new List<NotificationsViewModel>();

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

                var query = repository.Add(entity);

                addedNotifications.Add(repository.ToViewModel(query));
            }

            return addedNotifications;
        }

        public static bool DeleteOldNotifications(this INotificationsRepository repository)
        {
            var DateThreshold = DateTime.Now.AddDays(-30);
            var query = repository.GetAll()
                                  .Where(x => x.Timestamp < DateThreshold)
                                  .ToList();

            repository.DelMany(query);
            return true;
        }
    }
}
