using System;
using Domain.Enums;
using Domain.Models;

namespace Domain.ViewModels
{
    public class SessionViewModel : BaseViewModel
    {
        public ProblemViewModel Problem { get; set; }
        public SpecialistViewModel Specialist { get; set; }
        public SessionStatus Status { get; set; }
        public double Reward { get; set; }
        public DateTime Date { get; set; }
        public bool IsSpecialistClose { get; set; }
        public bool IsClientClose { get; set; }

        public SessionViewModel(Session session)
        {
            if (session != null)
            {
                ID = session.ID;
                Problem = new ProblemViewModel(session.Problem);
                Specialist = new SpecialistViewModel(session.Specialist);
                Status = session.Status;
                Reward = session.Reward;
                Date = session.Date;
                IsSpecialistClose = session.IsSpecialistClose;
                IsClientClose = session.IsClientClose;
            }
        }

        public SessionViewModel(Session session, SpecialistViewModel specialist)
        {
            if (session != null)
            {
                ID = session.ID;
                Problem = new ProblemViewModel(session.Problem);
                Specialist = specialist;
                Status = session.Status;
                Reward = session.Reward;
                Date = session.Date;
                IsSpecialistClose = session.IsSpecialistClose;
                IsClientClose = session.IsClientClose;
            }
        }
    }
}
