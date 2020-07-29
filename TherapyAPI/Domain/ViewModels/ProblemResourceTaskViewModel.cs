using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ProblemResourceTaskViewModel : BaseViewModel
    {
        public ProblemResourceViewModel Resource { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }

        public ProblemResourceTaskViewModel(ProblemResourceTask task)
        {
            if (task != null)
            {
                ID = task.ID;
                Resource = new ProblemResourceViewModel(task.Resource);
                Title = task.Title;
                IsDone = task.IsDone;
            }
        }
    }
}
