using System;
using System.Collections.Generic;

namespace Domain.ViewModels.Response
{
    public class ReviewsResponse : ResponseModel
    {
        public List<ReviewViewModel> PositiveReviews { get; set; }
        public List<ReviewViewModel> NeutralReviews { get; set; }
        public List<ReviewViewModel> NegativeReviews { get; set; }
        public double Rating { get; set; }
    }
}
