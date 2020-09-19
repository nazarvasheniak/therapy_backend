using System;
namespace Domain.Models
{
    public class Problem : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual string ProblemText { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
