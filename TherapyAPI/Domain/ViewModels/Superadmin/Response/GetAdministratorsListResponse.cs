using System;
using Domain.Enums;
using Domain.ViewModels.Response;

namespace Domain.ViewModels.Superadmin.Response
{
    public class GetAdministratorsListResponse : ListResponse<object>
    {
        public OrderBy OrderBy { get; set; }
        public string SearchQuery { get; set; }
    }
}
