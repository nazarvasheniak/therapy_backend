using System;
using Domain.Enums;
using Domain.ViewModels.Request;

namespace Domain.ViewModels.Superadmin.Request
{
    public class GetAdministratorsListRequest : GetList
    {
        public OrderBy OrderBy { get; set; }
        public string SearchQuery { get; set; }
    }
}
