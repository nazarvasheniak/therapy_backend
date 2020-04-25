using System;
namespace Domain.ViewModels.Response
{
    public class ResponseModel
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "OK";
    }
}
