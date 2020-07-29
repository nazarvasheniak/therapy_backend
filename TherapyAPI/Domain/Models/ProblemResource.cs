using System;
namespace Domain.Models
{
    public class ProblemResource : PersistentObject, IDeletableObject
    {
        public virtual Session Session { get; set; }
        public virtual string Title { get; set; }
        public virtual string Emotion { get; set; }
        public virtual string Location { get; set; }
        public virtual string Characteristic { get; set; }
        public virtual string Influence { get; set; }
        public virtual int LikeScore { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
