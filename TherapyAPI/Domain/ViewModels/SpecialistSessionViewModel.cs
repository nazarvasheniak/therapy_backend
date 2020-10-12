using System;
using Domain.Enums;

namespace Domain.ViewModels
{
    public class SpecialistSessionViewModel
    {
        public SpecialistViewModel Specialist { get; set; }
        public long SessionID { get; set; }
        public DateTime SessionDate { get; set; }
        public SessionStatus SessionStatus { get; set; }
        public UserViewModel Client { get; set; }
        public ProblemViewModel Problem { get; set; }
        public ReviewViewModel Review { get; set; }
        public string ProblemText { get; set; }
        public int ReviewScore { get; set; }
        public double Reward { get; set; }

        public bool IsSpecialistClose { get; set; }
        public bool IsClientClose { get; set; }

        public DateTime SpecialistCloseDate { get; set; }
        public DateTime ClientCloseDate { get; set; }

        public int SessionImagesCount { get; set; }
        public int TotalImagesCount { get; set; }

        public int SessionResourcesCount { get; set; }
        public int TotalResourcesCount { get; set; }

        public bool IsAllImagesFromOneSpecialist { get; set; }
        public bool IsAllResourcesFromOneSpecialist { get; set; }
    }
}
