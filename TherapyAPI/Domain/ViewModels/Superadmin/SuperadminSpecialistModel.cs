using System;
namespace Domain.ViewModels.Superadmin
{
    public class SuperadminSpecialistModel : SuperadminModel
    {
        public int TotalSessionsCount { get; set; }
        public double TotalEarned { get; set; }
        public double Rating { get; set; }
    }
}
