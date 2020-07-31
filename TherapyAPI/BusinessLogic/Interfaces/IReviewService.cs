using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;

namespace BusinessLogic.Interfaces
{
    public interface IReviewService : IBaseCrudService<Review>
    {
        double GetSpecialistRating(Specialist specialist);
        List<ReviewViewModel> GetSpecialistReviews(Specialist specialist, string type);
        Review GetSessionReview(Session session);
    }
}
