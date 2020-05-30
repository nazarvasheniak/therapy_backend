using System;
namespace Domain.Models
{
    public class Specialist : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual File Photo { get; set; }
        public virtual double Price { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
