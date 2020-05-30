﻿using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ProblemMap : ClassMap<Problem>
    {
        public ProblemMap()
        {
            Table("problems");
            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.ProblemText, "problem_text");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
