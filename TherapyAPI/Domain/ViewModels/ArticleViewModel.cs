using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ArticleViewModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public FileViewModel Image { get; set; }
        public string ShortText { get; set; }
        public string Text { get; set; }
        public SpecialistViewModel Author { get; set; }
        public DateTime Date { get; set; }
        public List<ArticleLikeViewModel> Likes { get; set; }
        public List<ArticleCommentViewModel> Comments { get; set; }
        public bool IsLiked { get; set; }

        public ArticleViewModel(Article article)
        {
            ID = article.ID;
            Title = article.Title;
            Image = new FileViewModel(article.Image);
            ShortText = article.ShortText;
            Text = Encoding.UTF8.GetString(article.Text);
            Author = new SpecialistViewModel(article.Author);
            Date = article.Date;
            Likes = new List<ArticleLikeViewModel>();
            Comments = new List<ArticleCommentViewModel>();
            IsLiked = false;
        }

        public ArticleViewModel(Article article, IEnumerable<ArticleLike> likes, IEnumerable<ArticleComment> comments, bool isLiked)
        {
            if (article != null)
            {
                ID = article.ID;
                Title = article.Title;
                Image = new FileViewModel(article.Image);
                ShortText = article.ShortText;
                Text = Encoding.UTF8.GetString(article.Text);
                Author = new SpecialistViewModel(article.Author);
                Date = article.Date;
                Likes = likes.Select(x => new ArticleLikeViewModel(x)).ToList();
                Comments = comments.Select(x => new ArticleCommentViewModel(x)).ToList();
                IsLiked = isLiked;
            }
        }

        public ArticleViewModel(Article article, IEnumerable<ArticleLike> likes, IEnumerable<ArticleCommentViewModel> comments, bool isLiked)
        {
            if (article != null)
            {
                ID = article.ID;
                Title = article.Title;
                Image = new FileViewModel(article.Image);
                ShortText = article.ShortText;
                Text = Encoding.UTF8.GetString(article.Text);
                Author = new SpecialistViewModel(article.Author);
                Date = article.Date;
                Likes = likes.Select(x => new ArticleLikeViewModel(x)).ToList();
                Comments = comments.ToList();
                IsLiked = isLiked;
            }
        }
    }
}
