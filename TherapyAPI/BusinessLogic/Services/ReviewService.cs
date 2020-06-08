using System;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
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
    }
}
