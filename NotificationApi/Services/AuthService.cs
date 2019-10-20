using MongoDB.Driver;
using NotificationApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;

        public AuthService(IDatabaseSettings dbsettings)
        {
            var client = new MongoClient(dbsettings.ConnectionString);
            var database = client.GetDatabase(dbsettings.DatabaseName);

            _users = database.GetCollection<User>("Users");


        }

        public User Create(User user)
        {
            _users.InsertOne(user);

            return user;
        }

        public User GetUser(string id)
        {
            var user=_users.Find<User>(u=>u.Id==id).FirstOrDefault();
            return user;
        }

        
    }
}
