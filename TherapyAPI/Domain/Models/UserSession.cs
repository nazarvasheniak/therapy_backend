using System;
namespace Domain.Models
{
    public class UserSession : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual string AuthCode { get; set; }
        public virtual string Token { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool Deleted { get; set; }   
    }
}
