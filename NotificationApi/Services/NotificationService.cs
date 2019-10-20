using MongoDB.Driver;
using NotificationApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<NotificationType> _notificationTypes;

        public NotificationService(IDatabaseSettings dbsettings)
        {
            var client = new MongoClient(dbsettings.ConnectionString);
            var database = client.GetDatabase(dbsettings.DatabaseName);

            _notificationTypes = database.GetCollection<NotificationType>("NotificationTypes");


        }

        public List<NotificationType> FindNotificationTypes()
        {
            var notificationTypes = _notificationTypes.Find<NotificationType>(_=>true).ToList();

            return notificationTypes;
        }

    }
}
