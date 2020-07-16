using System;
using Domain.Enums;

namespace Domain.ViewModels.Request
{
    public class CreatePaymentRequest
    {
        public int Amount { get; set; }
        public PaymentType Type { get; set; }
        public long SessionID { get; set; }
    }
}
