using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class SessionService : BaseCrudService<Session>, ISessionService
    {
        public SessionService(IRepository<Session> repository) : base(repository)
        {
        }

        public Session GetCurrentSession(Problem problem)
        {
            return GetAll().FirstOrDefault(x =>
                x.Problem == problem &&
                x.Status == SessionStatus.Started);
        }

        public List<Session> GetActiveSessions(User user)
        {
            return GetAll().Where(x => x.Problem.User == user && x.Status == SessionStatus.Started).ToList();
        }

        public Session GetNewActiveSession(Problem problem)
        {
            return GetAll().FirstOrDefault(x => x.Problem == problem && x.Status == SessionStatus.New);
        }

        public Session GetWaitingSession(Problem problem)
        {
            return GetAll().FirstOrDefault(x => x.Problem == problem && x.Status == SessionStatus.Waiting);
        }

        public List<Session> GetSpecialistSessions(Specialist specialist)
        {
            return GetAll().Where(x => x.Specialist == specialist).OrderByDescending(x => x.Date).ToList();
        }

        public List<User> GetSpecialistClients(Specialist specialist)
        {
            var list = new HashSet<User>();
            var sessions = GetAll()
                .Where(x => x.Specialist == specialist && x.Status != SessionStatus.Waiting)
                .ToList();

            sessions.ForEach(x => list.Add(x.Problem.User));

            return list.ToList();
        }

        public List<Session> GetUserSessions(User user)
        {
            return GetAll().Where(x => x.Problem.User == user).OrderByDescending(x  => x.Date).ToList();
        }

        public List<Session> GetProblemSessions(Problem problem)
        {
            return GetAll().Where(x => x.Problem == problem).ToList();
        }
    }
}
