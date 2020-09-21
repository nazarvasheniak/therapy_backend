using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("users");
            Id(u => u.ID, "id");

            References(e => e.Photo, "id_photo");

            Map(u => u.FirstName, "first_name");
            Map(u => u.LastName, "last_name");
            Map(u => u.PhoneNumber, "phone_num");
            Map(u => u.Email, "email");
            Map(u => u.Role, "role").CustomType<UserRole>();
            Map(u => u.RegisteredAt, "registered_at");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
