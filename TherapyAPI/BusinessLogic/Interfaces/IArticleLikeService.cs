using System;
using System.Collections.Generic;
using Domain.Models;
using Domain.ViewModels;

namespace BusinessLogic.Interfaces
{
    public interface IArticleLikeService : IBaseCrudService<ArticleLike>
    {
        List<ArticleLikeViewModel> GetArticleLikes(Article article);
        long GetArticleLikesCount(Article article);
    }
}
