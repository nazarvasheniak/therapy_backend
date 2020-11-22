using System;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ArticlePublishService : BaseCrudService<ArticlePublish>, IArticlePublishService
    {
        public ArticlePublishService(IRepository<ArticlePublish> repository) : base(repository)
        {
        }

        public ArticlePublish GetArticlePublish(long articleID)
        {
            return GetAll().FirstOrDefault(publish => publish.Article.ID == articleID);
        }

        public ArticlePublish GetArticlePublish(Article article)
        {
            return GetAll().FirstOrDefault(publish => publish.Article == article);
        }
    }
}
