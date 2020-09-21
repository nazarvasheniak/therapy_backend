using System;
using Domain.Enums;
using Domain.ViewModels;

namespace Domain.Models
{
    public class User : PersistentObject, IDeletableObject
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public virtual File Photo { get; set; }
        public virtual UserRole Role { get; set; }
        public virtual DateTime RegisteredAt { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
