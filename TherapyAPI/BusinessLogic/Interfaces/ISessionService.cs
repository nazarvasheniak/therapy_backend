using System;
using System.Collections.Generic;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface ISessionService : IBaseCrudService<Session>
    {
        List<Session> GetActiveSessions(User user);
        Session GetWaitingSession(Problem problem);
        List<Session> GetSpecialistSessions(Specialist specialist);
        List<User> GetSpecialistClients(Specialist specialist);
    }
}
