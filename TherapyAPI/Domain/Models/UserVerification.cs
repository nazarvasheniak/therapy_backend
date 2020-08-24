using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class UserVerification : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual UserVerificationRequest VerificationRequest { get; set; }
        public virtual File Document { get; set; }
        public virtual File Selfie { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
