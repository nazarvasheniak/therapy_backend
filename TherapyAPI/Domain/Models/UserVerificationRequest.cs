using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class UserVerificationRequest : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual File Document { get; set; }
        public virtual File Selfie { get; set; }
        public virtual UserVerificationRequestStatus Status { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
