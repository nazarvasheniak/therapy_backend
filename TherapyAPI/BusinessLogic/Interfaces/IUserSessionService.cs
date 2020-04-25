using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IUserSessionService : IBaseCrudService<UserSession>
    {
        UserSession CreateSession(User user);
    }
}
