using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TherapyAPI.BackgroundServices
{
    public class SessionsTimerService
    {
        private ISessionService SessionService { get; set; }
        private IUserWalletService UserWalletService { get; set; }

        public SessionsTimerService()
        {

        }

        public void SetServices(
            ISessionService sessionService,
            IUserWalletService userWalletService)
        {
            if (SessionService == null)
                SessionService = sessionService;

            if (UserWalletService == null)
                UserWalletService = userWalletService;

            StartTimers();
        }

        private void StartTimers()
        {
            var sessions = GetActiveSessions();
            sessions.ForEach(session =>
            {
                var endTime = session.SpecialistCloseDate - DateTime.UtcNow;
                Task.Delay(endTime).ContinueWith(o => CloseSession(session));
            });
        }

        private List<Session> GetActiveSessions()
        {
            return SessionService.GetAll()
                .Where(x => x.IsSpecialistClose && !x.IsClientClose && (x.SpecialistCloseDate > DateTime.UtcNow))
                .ToList();
        }

        private void CloseSession(Session session)
        {
            var clientWallet = UserWalletService.GetUserWallet(session.Problem.User);
            var specialistWallet = UserWalletService.GetUserWallet(session.Specialist.User);

            clientWallet.Balance -= session.Reward;
            specialistWallet.Balance += session.Reward;

            session.IsClientClose = true;
            session.Status = SessionStatus.Success;
            session.ClientCloseDate = DateTime.UtcNow;

            SessionService.Update(session);
            UserWalletService.Update(clientWallet);
            UserWalletService.Update(specialistWallet);
        }
    }
}
