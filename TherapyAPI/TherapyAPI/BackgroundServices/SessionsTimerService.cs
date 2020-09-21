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

            StartTimers();
        }

        private void StartTimers()
        {
            var sessions = GetActiveSessions();
            sessions.ForEach(session =>
            {
                try
                {
                    var endTime = session.SpecialistCloseDate.AddDays(1) - DateTime.UtcNow;
                    Task.Delay(endTime).ContinueWith(o => CloseSession(session));
                } catch
                {

                }
            });
        }

        private List<Session> GetActiveSessions()
        {
            return SessionService.GetAll()
                .Where(x => x.IsSpecialistClose && !x.IsClientClose && (x.SpecialistCloseDate < DateTime.UtcNow))
                .ToList();
        }

        private async Task CloseSession(Session session)
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

            var clientActiveSessions = SessionService.GetActiveSessions(session.Problem.User);

            await NotificationsService
                .SendUpdateToUser(
                    session.Problem.User.ID,
                    SocketMessageType.ProblemSessionUpdate,
                    session);

            await NotificationsService
                .SendUpdateToUser(
                    session.Problem.User.ID,
                    SocketMessageType.BalanceUpdate,
                    new UserWalletViewModel(clientWallet, clientActiveSessions.Sum(x => x.Reward)));

            await NotificationsService.SendUpdateToUser(
                session.Specialist.ID,
                SocketMessageType.BalanceUpdate,
                new UserWalletViewModel(specialistWallet, 0));
        }
    }
}
