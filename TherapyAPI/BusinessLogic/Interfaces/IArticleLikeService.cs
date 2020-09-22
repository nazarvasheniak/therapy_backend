using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IArticleLikeService : IBaseCrudService<ArticleLike>
    {
        long GetArticleLikesCount(Article article);
    }
}
