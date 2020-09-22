using System;
using Domain.Enums;

namespace Domain.ViewModels.Request
{
    public class GetArticlesList : GetList
    {
        public ArticlesSort SortBy { get; set; }
        public OrderBy OrderBy { get; set; }
    }
}
