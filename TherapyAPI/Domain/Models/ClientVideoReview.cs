using System;
namespace Domain.Models
{
    public class ClientVideoReview : PersistentObject, IDeletableObject
    {
        public virtual string FullName { get; set; }
        public virtual File Photo { get; set; }
        public virtual string Text { get; set; }
        public virtual string LinkVK { get; set; }
        public virtual string LinkYouTube { get; set; }
        public virtual DateTime ReviewDate { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
