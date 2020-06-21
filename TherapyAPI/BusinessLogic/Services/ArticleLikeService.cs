using System;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ArticleLikeService : BaseCrudService<ArticleLike>, IArticleLikeService
    {
        public ArticleLikeService(IRepository<ArticleLike> repository) : base(repository)
        {
        }
    }
}
