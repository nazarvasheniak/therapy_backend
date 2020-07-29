using System;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ProblemImageMap : ClassMap<ProblemImage>
    {
        public ProblemImageMap()
        {
            Table("problems_images");
            Id(u => u.ID, "id");

            References(e => e.Session, "id_session");
            References(e => e.ParentImage, "id_parent_image");

            Map(u => u.Title, "image_title");
            Map(u => u.Emotion, "image_emotion");
            Map(u => u.Location, "image_location");
            Map(u => u.Characteristic, "image_characteristic");
            Map(u => u.IsMine, "is_mine");
            Map(u => u.IsIDo, "is_i_do");
            Map(u => u.IsForever, "is_forever");
            Map(u => u.LikeScore, "like_score");
            Map(u => u.IsHidden, "is_hidden").Not.Nullable();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
