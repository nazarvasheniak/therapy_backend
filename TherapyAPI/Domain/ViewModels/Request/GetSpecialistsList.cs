using System;
using Domain.Enums;

namespace Domain.ViewModels.Request
{
    public class GetSpecialistsList : GetList
    {
        public SpecialistsSort SortBy { get; set; }
        public OrderBy OrderBy { get; set; }
    }
}
