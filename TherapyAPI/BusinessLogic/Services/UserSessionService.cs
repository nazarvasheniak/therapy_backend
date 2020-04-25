using System;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class UserSessionService : BaseCrudService<UserSession>, IUserSessionService
    {
        public UserSessionService(IRepository<UserSession> repository) : base(repository)
        {
        }

        public UserSession CreateSession(User user)
        {
            var session = new UserSession
            {
                User = user,
                AuthCode = RandomNumber().ToString(),
                Created = DateTime.UtcNow
            };

            Create(session);

            return session;
        }

        private int RandomNumber()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }
}
