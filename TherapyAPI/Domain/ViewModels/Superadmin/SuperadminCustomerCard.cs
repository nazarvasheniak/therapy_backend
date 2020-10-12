using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.ViewModels.Superadmin
{
    public class SuperadminCustomerCard
    {
        public long UserID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public List<SpecialistSessionViewModel> Sessions { get; set; }
        public int ProblemsCount { get; set; }
        public int RefundsCount { get; set; }
        public double SpendOrEarned { get; set; }
    }
}
