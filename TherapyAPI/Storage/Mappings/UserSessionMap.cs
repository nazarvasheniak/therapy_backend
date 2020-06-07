using System;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class UserSessionMap : ClassMap<UserSession>
    {
        public UserSessionMap()
        {
            Table("users_sessions");
            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.AuthCode, "auth_code");
            Map(u => u.Token, "token").CustomSqlType("text").Length(1994967295);
            Map(u => u.Created, "created");
            Map(u => u.IsActive, "is_active");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
