using System;
using Domain.Enums;

namespace Domain.Models
{
    public class File : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual FileType Type { get; set; }
        public virtual string Url { get; set; }
        public virtual string LocalPath { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
