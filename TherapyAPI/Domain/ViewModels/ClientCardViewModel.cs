using System;
using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class ClientCardViewModel : BaseViewModel
    {
        public UserViewModel User { get; set; }
        public List<SpecialistSessionViewModel> Sessions { get; set; }
        public int ProblemsCount { get; set; }
        public double Paid { get; set; }
        public double RefundsCount { get; set; }
        public double AverageScore { get; set; }
    }
}
