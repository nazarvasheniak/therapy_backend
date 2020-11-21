using System;
using Domain.ViewModels.Response;

namespace Domain.ViewModels.Superadmin.Response
{
    public class GetSearchListResponse : ListResponse<object>
    {
        public string SearchQuery { get; set; }
    }
}
