using System;
using Domain.Enums;

namespace Domain.ViewModels.Response
{
    public class SpecialistsListResponse : ListResponse<SpecialistViewModel>
    {
        public SpecialistsSort SortBy { get; set; }
        public OrderBy OrderBy { get; set; }
    }
}
