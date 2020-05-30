using System;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class SessionService : BaseCrudService<Session>, ISessionService
    {
        public SessionService(IRepository<Session> repository) : base(repository)
        {
        }
    }
}
