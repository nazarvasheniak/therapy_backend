using System;
namespace Domain.ViewModels
{
    public class SpecialistProfileViewModel
    {
        public long SpecialistID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Rating { get; set; }
        public int PositiveReviewsCount { get; set; }
        public int NeutralReviewsCount { get; set; }
        public int NegativeReviewsCount { get; set; }
        public string PhotoUrl { get; set; }
        public double Price { get; set; }
        public double TotalEarnings { get; set; }
        public int TotalSessions { get; set; }
        public int TotalClients { get; set; }
    }
}
