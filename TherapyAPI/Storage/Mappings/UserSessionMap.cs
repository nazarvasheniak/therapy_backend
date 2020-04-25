using System;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class UserSessionMap : ClassMap<UserSession>
    {
        public UserSessionMap()
        {
            Table("user_sessions");
            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.AuthCode, "auth_code");
            Map(u => u.Token, "token");
            Map(u => u.Created, "created");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
