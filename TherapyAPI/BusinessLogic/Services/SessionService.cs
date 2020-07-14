﻿using System;
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

        public List<Session> GetActiveSessions(User user)
        {
            return GetAll().Where(x => x.Problem.User == user && x.Status == SessionStatus.Started).ToList();
        }
    }
}
