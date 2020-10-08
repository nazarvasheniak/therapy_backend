using System;
using System.Collections.Generic;

namespace Domain.ViewModels.Response
{
    public class ListResponse<T> : ResponseModel
    {
        public List<T> Data { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
