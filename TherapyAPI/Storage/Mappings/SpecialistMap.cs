﻿using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class SpecialistMap : ClassMap<Specialist>
    {
        public SpecialistMap()
        {
            Table("specialists");
            Id(u => u.ID, "id");

            References(e => e.User, "id_user");
            References(e => e.Photo, "id_photo");

            Map(u => u.Price, "price");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
