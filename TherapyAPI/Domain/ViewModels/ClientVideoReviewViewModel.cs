using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ClientVideoReviewViewModel : BaseViewModel
    {
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
        public string Text { get; set; }
        public string LinkVK { get; set; }
        public string LinkYouTube { get; set; }
        public DateTime ReviewDate { get; set; }

        public ClientVideoReviewViewModel(ClientVideoReview review)
        {
            if (review != null)
            {
                ID = review.ID;
                FullName = review.FullName;
                Text = review.Text;
                LinkVK = review.LinkVK;
                LinkYouTube = review.LinkYouTube;
                ReviewDate = review.ReviewDate;
            }
        }
    }
}
