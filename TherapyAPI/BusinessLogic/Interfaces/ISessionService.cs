using System;
using System.Collections.Generic;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface ISessionService : IBaseCrudService<Session>
    {
        Session GetCurrentSession(Problem problem);
        List<Session> GetActiveSessions(User user);
        Session GetNewActiveSession(Problem problem);
        Session GetWaitingSession(Problem problem);
        List<Session> GetSpecialistSessions(Specialist specialist);
        List<User> GetSpecialistClients(Specialist specialist);
        List<Session> GetUserSessions(User user);
        List<Session> GetProblemSessions(Problem problem);
    }
}
