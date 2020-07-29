using System;
namespace Domain.Models
{
    public class ProblemResourceTask : PersistentObject, IDeletableObject
    {
        public virtual ProblemResource Resource { get; set; }
        public virtual string Title { get; set; }
        public virtual bool IsDone { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
