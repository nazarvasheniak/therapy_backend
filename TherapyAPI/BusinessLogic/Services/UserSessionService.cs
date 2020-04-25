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
    }
}
