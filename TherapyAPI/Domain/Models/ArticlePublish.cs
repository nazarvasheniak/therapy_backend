using System;
using Domain.Enums;

namespace Domain.Models
{
    public class ArticlePublish : PersistentObject, IDeletableObject
    {
        public virtual Article Article { get; set; }
        public virtual ArticleModerationStatus Status { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
