using System;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class PaymentService : BaseCrudService<Payment>, IPaymentService
    {
        public PaymentService(IRepository<Payment> repository) : base(repository)
        {
        }

        public Payment GetPaymentByOrderID(string orderID)
        {
            return GetAll().FirstOrDefault(x => x.OrderID == orderID);
        }
    }
}
