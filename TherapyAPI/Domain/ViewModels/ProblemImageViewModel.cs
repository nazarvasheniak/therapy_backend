using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ProblemImageViewModel : BaseViewModel
    {
        public SessionViewModel Session { get; set; }
        public ProblemImageViewModel ParentImage { get; set; }
        public string Title { get; set; }
        public string Emotion { get; set; }
        public string Location { get; set; }
        public string Characteristic { get; set; }
        public bool IsMine { get; set; }
        public bool IsIDo { get; set; }
        public bool IsForever { get; set; }
        public bool IsHidden { get; set; }
        public int LikeScore { get; set; }

        public ProblemImageViewModel(ProblemImage image)
        {
            if (image != null)
            {
                ID = image.ID;
                Session = new SessionViewModel(image.Session);

                if (image.ParentImage != null)
                    ParentImage = new ProblemImageViewModel(image.ParentImage);

                Title = image.Title;
                Emotion = image.Emotion;
                Location = image.Location;
                Characteristic = image.Characteristic;
                IsMine = image.IsMine;
                IsIDo = image.IsIDo;
                IsForever = image.IsForever;
                LikeScore = image.LikeScore;
                IsHidden = image.IsHidden;
            }
        }
    }
}
