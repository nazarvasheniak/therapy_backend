using Domain.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Mappings
{
    public class UserVerificationMap : ClassMap<UserVerification>
    {
        public UserVerificationMap()
        {
            Table("users_verifications");
            Id(u => u.ID, "id");

            References(e => e.User, "id_user");
            References(e => e.Document, "id_document");
            References(e => e.Selfie, "id_selfie");
            References(e => e.VerificationRequest, "id_request").Not.Nullable();

            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
