using System;
namespace Domain.Models
{
    public class ArticleLike : PersistentObject, IDeletableObject
    {
        public virtual User Author { get; set; }
        public virtual Article Article { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
