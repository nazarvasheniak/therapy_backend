using System;
using System.Linq;
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

        public long GetArticleLikesCount(Article article)
        {
            return GetAll().Where(x => x.Article == article).ToList().Count;
        }
    }
}
