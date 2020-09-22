using System;
using System.Collections.Generic;
using Domain.Models;
using Domain.ViewModels;

namespace BusinessLogic.Interfaces
{
    public interface IArticleCommentService : IBaseCrudService<ArticleComment>
    {
        List<ArticleCommentViewModel> GetArticleComments(Article article);
        List<ArticleCommentViewModel> GetCommentReplies(ArticleComment comment);
        long GetArticleCommentsCount(Article article);
    }
}
