using System;
namespace Domain.Models
{
    public class Review : PersistentObject, IDeletableObject
    {
        public virtual int Score { get; set; }
        public virtual string Text { get; set; }
        public virtual Session Session { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
