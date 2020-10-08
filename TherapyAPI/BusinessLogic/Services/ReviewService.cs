using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ReviewService : BaseCrudService<Review>, IReviewService
    {
        public ReviewService(IRepository<Review> repository) : base(repository)
        {
        }

        public double GetSpecialistRating(Specialist specialist)
        {
            var reviews = GetAll().Where(x => x.Session.Specialist == specialist).ToList();

            if (reviews.Count == 0)
                return 0;

            var scoreSum = reviews.Sum(x => x.Score);
            double result = scoreSum / reviews.Count;

            return result;
        }

        public List<ReviewViewModel> GetSpecialistReviews(Specialist specialist, string type)
        {
            var t = Enum.Parse<ReviewType>(type);
                
            switch (t)
            {
                case ReviewType.Positive:
                    return GetAll()
                        .Where(x => x.Session.Specialist == specialist && x.Score > 3)
                        .OrderByDescending(x => x.Session.Date)
                        .Select(x => new ReviewViewModel(x))
                        .ToList();

                case ReviewType.Neutral:
                    return GetAll()
                        .Where(x => x.Session.Specialist == specialist && x.Score == 3)
                        .OrderByDescending(x => x.Session.Date)
                        .Select(x => new ReviewViewModel(x))
                        .ToList();

                case ReviewType.Negative:
                    return GetAll()
                        .Where(x => x.Session.Specialist == specialist && x.Score < 3)
                        .OrderByDescending(x => x.Session.Date)
                        .Select(x => new ReviewViewModel(x))
                        .ToList();

                default:
                    return GetAll()
                        .Where(x => x.Session.Specialist == specialist)
                        .OrderByDescending(x => x.Session.Date)
                        .Select(x => new ReviewViewModel(x))
                        .ToList();
            }
        }

        public Review GetSessionReview(Session session)
        {
            return GetAll().FirstOrDefault(x => x.Session == session);
        }

        public double GetUserAverageScore(User user)
        {
            var reviews = GetAll().Where(x => x.Session.Problem.User == user).ToList();
            if (reviews == null || reviews.Count == 0)
                return 0;

            var average = reviews.Average(x => x.Score);

            return average;
        }
    }
}
