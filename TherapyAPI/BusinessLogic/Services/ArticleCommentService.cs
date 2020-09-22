using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Domain.ViewModels;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ArticleCommentService : BaseCrudService<ArticleComment>, IArticleCommentService
    {
        public ArticleCommentService(IRepository<ArticleComment> repository) : base(repository)
        {
        }

        public List<ArticleCommentViewModel> GetArticleComments(Article article)
        {
            var result = new List<ArticleCommentViewModel>();
            var comments = GetAll().Where(x => x.Article == article).ToList();

            comments.ForEach(comment =>
            {
                var replies = GetCommentReplies(comment);
                result.Add(new ArticleCommentViewModel(comment, replies));
            });

            return result;
        }

        public List<ArticleCommentViewModel> GetCommentReplies(ArticleComment comment)
        {
            return GetAll().Where(x => x.IsReply && x.ParentComment == comment)
                .Select(x => new ArticleCommentViewModel(x, GetCommentReplies(x)))
                .ToList();
        }

        public long GetArticleCommentsCount(Article article)
        {
            long result = 0;

            var comments = GetAll().Where(x => x.Article == article).ToList();
            result += comments.Count;

            comments.ForEach(comment =>
            {
                var replies = GetCommentReplies(comment);
                result += replies.Count;
            });

            return result;
        }
    }
}
