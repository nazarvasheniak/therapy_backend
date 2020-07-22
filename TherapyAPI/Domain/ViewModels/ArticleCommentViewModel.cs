using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ArticleCommentViewModel : BaseViewModel
    {
        public UserViewModel Author { get; set; }
        public string Text { get; set; }
        public bool IsReply { get; set; }
        public ArticleCommentViewModel ParentComment { get; set; }
        public List<ArticleCommentViewModel> Replies { get; set; }
        public DateTime Date { get; set; }

        public ArticleCommentViewModel(ArticleComment comment)
        {
            if (comment != null)
            {
                ID = comment.ID;
                Author = new UserViewModel(comment.Author);
                Text = comment.Text;
                IsReply = comment.IsReply;
                ParentComment = new ArticleCommentViewModel(comment.ParentComment);
                Date = comment.Date;
            }
        }

        public ArticleCommentViewModel(ArticleComment comment, List<ArticleComment> replies)
        {
            if (comment != null)
            {
                ID = comment.ID;
                Author = new UserViewModel(comment.Author);
                Text = comment.Text;
                IsReply = comment.IsReply;
                ParentComment = new ArticleCommentViewModel(comment.ParentComment);
                Replies = replies.Select(x => new ArticleCommentViewModel(x)).ToList();
                Date = comment.Date;
            }
        }

        public ArticleCommentViewModel(ArticleComment comment, List<ArticleCommentViewModel> replies)
        {
            if (comment != null)
            {
                ID = comment.ID;
                Author = new UserViewModel(comment.Author);
                Text = comment.Text;
                IsReply = comment.IsReply;
                ParentComment = new ArticleCommentViewModel(comment.ParentComment);
                Replies = replies;
                Date = comment.Date;
            }
        }
    }
}
