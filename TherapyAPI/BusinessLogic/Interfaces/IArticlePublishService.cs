using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IArticlePublishService : IBaseCrudService<ArticlePublish>
    {
        ArticlePublish GetArticlePublish(long articleID);
        ArticlePublish GetArticlePublish(Article article);
    }
}
