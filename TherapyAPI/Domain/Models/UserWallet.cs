using System;
namespace Domain.Models
{
    public class UserWallet : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual double Balance { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
