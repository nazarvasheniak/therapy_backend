using System;
using System.Collections.Generic;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IArticleService : IBaseCrudService<Article>
    {
        List<Article> GetAllArticles();
        bool IsArticleExist(string title);
    }
}
