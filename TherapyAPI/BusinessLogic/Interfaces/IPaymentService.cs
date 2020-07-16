using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IPaymentService : IBaseCrudService<Payment>
    {
        Payment GetPaymentByOrderID(string orderID);
    }
}
