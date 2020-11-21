using System;
using Domain.ViewModels.Request;

namespace Domain.ViewModels.Superadmin.Request
{
    public class GetSearchListRequest : GetList
    {
        public string SearchQuery { get; set; }
    }
}
