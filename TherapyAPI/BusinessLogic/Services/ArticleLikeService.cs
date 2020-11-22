using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Domain.ViewModels;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ArticleLikeService : BaseCrudService<ArticleLike>, IArticleLikeService
    {
        public ArticleLikeService(IRepository<ArticleLike> repository) : base(repository)
        {
        }

        public List<ArticleLikeViewModel> GetArticleLikes(Article article)
        {
            return GetAll().Where(x => x.Article == article)
                .Select(x => new ArticleLikeViewModel(x))
                .ToList();
        }

        public long GetArticleLikesCount(Article article)
        {
            return GetAll().Where(x => x.Article == article).ToList().Count;
        }
    }
}
