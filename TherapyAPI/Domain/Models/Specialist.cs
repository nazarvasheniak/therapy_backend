using System;
using System.Collections.Generic;
using Domain.ViewModels;

namespace Domain.Models
{
    public class Specialist : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual double Price { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
