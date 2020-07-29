using System;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ProblemResourceMap : ClassMap<ProblemResource>
    {
        public ProblemResourceMap()
        {
            Table("problems_resources");
            Id(u => u.ID, "id");

            References(e => e.Session, "id_session");

            Map(u => u.Title, "resource_title");
            Map(u => u.Emotion, "resource_emotion");
            Map(u => u.Location, "resource_location");
            Map(u => u.Characteristic, "resource_characteristic");
            Map(u => u.Influence, "resource_influence");
            Map(u => u.LikeScore, "like_score");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
