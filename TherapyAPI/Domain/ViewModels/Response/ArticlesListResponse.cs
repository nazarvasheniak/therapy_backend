using System;
using Domain.Enums;

namespace Domain.ViewModels.Response
{
    public class ArticlesListResponse : ListResponse<ArticleViewModel>
    {
        public ArticlesSort SortBy { get; set; }
        public OrderBy OrderBy { get; set; }
    }
}
