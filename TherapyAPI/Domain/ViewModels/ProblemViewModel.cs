using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ProblemViewModel : BaseViewModel
    {
        public UserViewModel User { get; set; }
        public string ProblemText { get; set; }
        public DateTime CreatedDate { get; set; }

        public ProblemViewModel(Problem problem)
        {
            if (problem != null)
            {
                ID = problem.ID;
                ProblemText = problem.ProblemText;
                User = new UserViewModel(problem.User);
                CreatedDate = problem.CreatedDate;
            }
        }
    }
}
