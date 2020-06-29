using System;

namespace Domain.Models
{
    public class ArticleComment : PersistentObject, IDeletableObject
    {
        public virtual User Author { get; set; }
        public virtual Article Article { get; set; }
        public virtual string Text { get; set; }
        public virtual bool IsReply { get; set; }
        public virtual ArticleComment ParentComment { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
