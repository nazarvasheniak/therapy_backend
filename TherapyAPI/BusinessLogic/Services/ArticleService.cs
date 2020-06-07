using System;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ArticleService : BaseCrudService<Article>, IArticleService
    {
        public ArticleService(IRepository<Article> repository) : base(repository)
        {
        }
    }
}
