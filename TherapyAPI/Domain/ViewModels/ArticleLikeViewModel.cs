using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ArticleLikeViewModel
    {
        public long ID { get; set; }
        public UserViewModel Author { get; set; }
        public ArticleViewModel Article { get; set; }

        public ArticleLikeViewModel(ArticleLike like)
        {
            if (like != null)
            {
                ID = like.ID;
                Author = new UserViewModel(like.Author);
                Article = new ArticleViewModel(like.Article);
            }
        }
    }
}
