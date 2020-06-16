using System;
using System.Linq;
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

        public bool IsArticleExist(string title)
        {
            var article = GetAll().FirstOrDefault(x => x.Title == title);

            if (article == null)
                return false;

            return true;
        }
    }
}
