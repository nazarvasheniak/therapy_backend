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
        public DateTime SpecialistCloseDate { get; set; }
        public DateTime ClientCloseDate { get; set; }
        public int ReviewScore { get; set; }

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
                SpecialistCloseDate = session.SpecialistCloseDate;
                ClientCloseDate = session.ClientCloseDate;
                ReviewScore = 0;
            }
        }

        public SessionViewModel(Session session, int reviewScore)
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
                SpecialistCloseDate = session.SpecialistCloseDate;
                ClientCloseDate = session.ClientCloseDate;
                ReviewScore = reviewScore;
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
                SpecialistCloseDate = session.SpecialistCloseDate;
                ClientCloseDate = session.ClientCloseDate;
                ReviewScore = 0;
            }
        }

        public SessionViewModel(Session session, SpecialistViewModel specialist, int reviewScore)
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
                SpecialistCloseDate = session.SpecialistCloseDate;
                ClientCloseDate = session.ClientCloseDate;
                ReviewScore = reviewScore;
            }
        }
    }
}
