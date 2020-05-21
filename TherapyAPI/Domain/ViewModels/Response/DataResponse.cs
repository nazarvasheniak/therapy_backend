using System;
namespace Domain.ViewModels.Response
{
    public class DataResponse<T> : ResponseModel
    {
        public T Data { get; set; }
    }
}
