using System;
using Domain.Enums;

namespace Domain.Models
{
    public class Session : PersistentObject, IDeletableObject
    {
        public virtual Problem Problem { get; set; }
        public virtual Specialist Specialist { get; set; }
        public virtual SessionStatus Status { get; set; }
        public virtual double Reward { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual bool IsSpecialistClose { get; set; }
        public virtual bool IsClientClose { get; set; }
        public virtual DateTime SpecialistCloseDate { get; set; }
        public virtual DateTime ClientCloseDate { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
