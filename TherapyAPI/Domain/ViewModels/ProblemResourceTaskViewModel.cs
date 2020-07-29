using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ProblemResourceTaskViewModel : BaseViewModel
    {
        public ProblemResourceViewModel Resource { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }

        public ProblemResourceTaskViewModel(ProblemResourceTaskViewModel item)
        {
            if (item != null)
            {
                ID = item.ID;
                Resource = item.Resource;
                Title = item.Title;
                IsDone = item.IsDone;
            }
        }

        public ProblemResourceTaskViewModel(ProblemResourceTask item)
        {
            if (item != null)
            {
                ID = item.ID;
                Resource = new ProblemResourceViewModel(item.Resource);
                Title = item.Title;
                IsDone = item.IsDone;
            }
        }
    }
}
