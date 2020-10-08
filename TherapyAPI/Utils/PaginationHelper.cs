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
        public static ListResponse<object> PaginateEntityCollection(IEnumerable<object> collection, GetList query)
        {
            return new ListResponse<object>
            {
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(collection.Count() / (double)query.PageSize),
                TotalItems = collection.ToList().Count,
                Data = collection
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList()
            };
        }
    }
}
