using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ProblemResourceViewModel : BaseViewModel
    {
        public SessionViewModel Session { get; set; }
        public string Title { get; set; }
        public string Emotion { get; set; }
        public string Location { get; set; }
        public string Characteristic { get; set; }
        public string Influence { get; set; }
        public int LikeScore { get; set; }

        public ProblemResourceViewModel(ProblemResource resource)
        {
            if (resource != null)
            {
                ID = resource.ID;
                Session = new SessionViewModel(resource.Session);
                Title = resource.Title;
                Emotion = resource.Emotion;
                Location = resource.Location;
                Characteristic = resource.Characteristic;
                Influence = resource.Influence;
                LikeScore = resource.LikeScore;
            }
        }
    }
}
