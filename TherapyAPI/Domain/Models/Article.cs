using System;
using Domain.Enums;

namespace Domain.Models
{
    public class Article : PersistentObject, IDeletableObject
    {
        public virtual string Title { get; set; }
        public virtual File Image { get; set; }
        public virtual string ShortText { get; set; }
        public virtual byte[] Text { get; set; }
        public virtual Specialist Author { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual ArticleModerationStatus ModerationStatus { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
