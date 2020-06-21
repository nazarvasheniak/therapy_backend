using System;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ArticleCommentService : BaseCrudService<ArticleComment>, IArticleCommentService
    {
        public ArticleCommentService(IRepository<ArticleComment> repository) : base(repository)
        {
        }
    }
}
