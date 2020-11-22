using System;
using Domain.Enums;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ArticlePublishViewModel : BaseViewModel
    {
        public virtual ArticleViewModel Article { get; set; }
        public virtual ArticleModerationStatus Status { get; set; }
        public virtual string Message { get; set; }

        public ArticlePublishViewModel(ArticlePublish publish)
        {
            if (publish != null)
            {
                ID = publish.ID;
                Article = new ArticleViewModel(publish.Article);
                Status = publish.Status;
                Message = publish.Message;
            }
        }

        public ArticlePublishViewModel(ArticlePublish publish, ArticleViewModel article)
        {
            if (publish != null)
            {
                ID = publish.ID;
                Article = article;
                Status = publish.Status;
                Message = publish.Message;
            }
        }
    }
}
