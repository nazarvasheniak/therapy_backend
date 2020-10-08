using System;
using Domain.Enums;
using Domain.ViewModels.Request;
using Domain.ViewModels.Superadmin.Enums;

namespace Domain.ViewModels.Superadmin.Request
{
    public class GetSpecialistsListRequest : GetList
    {
        public SpecialistsSorter SortBy { get; set; }
        public OrderBy OrderBy { get; set; }
        public string SearchQuery { get; set; }
    }
}
