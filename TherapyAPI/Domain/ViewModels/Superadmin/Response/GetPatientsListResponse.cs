using System;
using Domain.Enums;
using Domain.ViewModels.Response;
using Domain.ViewModels.Superadmin.Enums;

namespace Domain.ViewModels.Superadmin.Response
{
    public class GetPatientsListResponse : ListResponse<object>
    {
        public PatientsSorter SortBy { get; set; }
        public OrderBy OrderBy { get; set; }
        public string SearchQuery { get; set; }
    }
}
