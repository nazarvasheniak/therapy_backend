using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Domain.ViewModels;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;

namespace Utils
{
    public static class PaginationHelper
    {
        public static ListResponse<BaseViewModel> PaginateEntityCollection(IEnumerable<BaseViewModel> collection, GetList query)
        {
            return new ListResponse<BaseViewModel>
            {
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(collection.Count() / (double)query.PageSize),
                Data = collection
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList()
            };
        }
    }
}
