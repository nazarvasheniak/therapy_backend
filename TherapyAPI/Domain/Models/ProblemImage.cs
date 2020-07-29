using System;
namespace Domain.Models
{
    public class ProblemImage : PersistentObject, IDeletableObject
    {
        public virtual Session Session { get; set; }
        public virtual ProblemImage ParentImage { get; set; }
        public virtual string Title { get; set; }
        public virtual string Emotion { get; set; }
        public virtual string Location { get; set; }
        public virtual string Characteristic { get; set; }
        public virtual bool IsMine { get; set; }
        public virtual bool IsIDo { get; set; }
        public virtual bool IsForever { get; set; }
        public virtual int LikeScore { get; set; }
        public virtual bool IsHidden { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
