using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TherapyAPI.WebSocketManager;
using TherapyAPI.WebSocketManager.Enums;

namespace TherapyAPI.BackgroundServices
{
    public class SessionsTimerService
    {
        private ISessionService SessionService { get; set; }
        private IUserWalletService UserWalletService { get; set; }
        private NotificationsMessageHandler NotificationsService { get; set; }

        private List<Session> SessionsQuenue = new List<Session>();

        public SessionsTimerService()
        {

        }

        public void SetServices(
            ISessionService sessionService,
            IUserWalletService userWalletService,
            NotificationsMessageHandler notificationsService)
        {
            if (SessionService == null)
                SessionService = sessionService;

            if (UserWalletService == null)
                UserWalletService = userWalletService;

            if (NotificationsService == null)
                NotificationsService = notificationsService;

            SessionsQuenue.AddRange(GetActiveSessions());
            StartTimers();
        }

        public void AddSession(Session session)
        {
            SessionsQuenue.Add(session);
            StartSessionTimer(session);
        }

        private void StartTimers()
        {
            SessionsQuenue.ForEach(session => StartSessionTimer(session));
        }

        private void StartSessionTimer(Session session)
        {
            var endTime = session.SpecialistCloseDate.AddDays(1) - DateTime.UtcNow;
            Task.Delay(endTime).ContinueWith(o => CloseSession(session));
        }

        private List<Session> GetActiveSessions()
        {
            return SessionService.GetAll()
                .Where(x => x.IsSpecialistClose && !x.IsClientClose)
                .ToList();
        }

        private void CloseSession(Session session)
        {
            var renewSession = SessionService.Get(session.ID);
            if (renewSession.IsClientClose)
            {
                SessionsQuenue.Remove(session);
                return;
            }

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

            SessionsQuenue.Remove(session);
        }
    }
}
