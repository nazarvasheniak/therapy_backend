using System;
using Domain.Enums;
using Domain.Models;

namespace Domain.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        public long Amount { get; set; }
        public string OrderID { get; set; }
        public PaymentType Type { get; set; }
        public PaymentStatus Status { get; set; }

        public PaymentViewModel(Payment payment)
        {
            if (payment != null)
            {
                ID = payment.ID;
                Amount = payment.Amount;
                OrderID = payment.OrderID;
                Type = payment.Type;
                Status = payment.Status;
            }
        }
    }
}
