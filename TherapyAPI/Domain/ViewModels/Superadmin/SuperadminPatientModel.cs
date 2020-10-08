using System;
using Domain.Enums;

namespace Domain.ViewModels.Superadmin
{
    public class SuperadminPatientModel : SuperadminModel
    {
        public int TotalSessionsCount { get; set; }
        public int TotalRefunds { get; set; }
        public double TotalPaid { get; set; }
        public double AverageScore { get; set; }
    }
}
