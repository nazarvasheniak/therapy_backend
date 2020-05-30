using System;
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
    }
}
