using System;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ClientVideoReviewService : BaseCrudService<ClientVideoReview>, IClientVideoReviewService
    {
        public ClientVideoReviewService(IRepository<ClientVideoReview> repository) : base(repository)
        {
        }
    }
}
