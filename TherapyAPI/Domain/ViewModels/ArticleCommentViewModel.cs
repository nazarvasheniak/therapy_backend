using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ArticleCommentViewModel
    {
        public long ID { get; set; }
        public UserViewModel Author { get; set; }
        public ArticleViewModel Article { get; set; }
        public string Text { get; set; }
        public bool IsReply { get; set; }
        public ArticleCommentViewModel ParentComment { get; set; }

        public ArticleCommentViewModel(ArticleComment comment)
        {
            if (comment != null)
            {
                ID = comment.ID;
                Author = new UserViewModel(comment.Author);
                Article = new ArticleViewModel(comment.Article);
                Text = comment.Text;
                IsReply = comment.IsReply;
                ParentComment = new ArticleCommentViewModel(comment.ParentComment);
            }
        }
    }
}
