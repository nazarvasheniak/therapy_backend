using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IArticleService : IBaseCrudService<Article>
    {
        bool IsArticleExist(string title);
    }
}
