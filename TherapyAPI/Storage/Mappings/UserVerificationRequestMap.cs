using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Mappings
{
    public class UserVerificationRequestMap : ClassMap<UserVerificationRequest>
    {
        public UserVerificationRequestMap()
        {
            Table("users_verification_requests");
            Id(u => u.ID, "id");

            References(e => e.User, "id_user");
            References(e => e.Document, "id_document");
            References(e => e.Selfie, "id_selfie");

            Map(u => u.Status, "request_status").CustomType<UserVerificationRequestStatus>();
            Map(u => u.CreatedAt, "created_at");
            Map(u => u.UpdatedAt, "updated_at");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
