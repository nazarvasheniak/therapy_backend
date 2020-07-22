using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ReviewViewModel : BaseViewModel
    {
        public int Score { get; set; }
        public string Text { get; set; }
        public SessionViewModel Session { get; set; }

        public ReviewViewModel(Review review)
        {
            if (review != null)
            {
                ID = review.ID;
                Score = review.Score;
                Text = review.Text;
                Session = new SessionViewModel(review.Session); 
            }
        }
    }
}
