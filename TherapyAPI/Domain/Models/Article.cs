﻿using System;
namespace Domain.Models
{
    public class Article : PersistentObject, IDeletableObject
    {
        public virtual string Title { get; set; }
        public virtual File Image { get; set; }
        public virtual string ShortText { get; set; }
        public virtual string Text { get; set; }
        public virtual User Author { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual bool Deleted { get; set; }
    }
}