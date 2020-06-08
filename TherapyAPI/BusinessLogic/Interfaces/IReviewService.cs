using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IReviewService : IBaseCrudService<Review>
    {
        double GetSpecialistRating(Specialist specialist);
    }
}
