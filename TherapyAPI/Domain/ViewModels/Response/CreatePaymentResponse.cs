using System;
namespace Domain.ViewModels.Response
{
    public class CreatePaymentResponse : ResponseModel
    {
        public string RedirectUrl { get; set; }
    }
}
