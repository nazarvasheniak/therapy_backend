using System;
using Domain.Enums;

namespace Domain.Models
{
    public class Payment : PersistentObject, IDeletableObject
    {
        public virtual UserWallet Wallet { get; set; }
        public virtual long Amount { get; set; }
        public virtual string OrderID { get; set; }
        public virtual PaymentType Type { get; set; }
        public virtual PaymentStatus Status { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
