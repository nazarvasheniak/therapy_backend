using System;
using Domain.Enums;

namespace Domain.ViewModels.Request
{
    public class GetReviews : GetList
    {
        public string Type { get; set; }
    }
}
