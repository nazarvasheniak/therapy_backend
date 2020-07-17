using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ReviewMap : ClassMap<Review>
    {
        public ReviewMap()
        {
            Table("reviews");
            Id(u => u.ID, "id");

            References(e => e.Session, "id_session");

            Map(u => u.Score, "score");
            Map(u => u.Text, "review_text").CustomSqlType("text").Length(65535);
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
