using System;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ClientVideoReviewMap : ClassMap<ClientVideoReview>
    {
        public ClientVideoReviewMap()
        {
            Table("clients_video_reviews");
            Id(u => u.ID, "id");

            References(e => e.Photo, "id_photo");

            Map(u => u.FullName, "full_name");
            Map(u => u.Text, "review_text").CustomSqlType("text").Length(150);
            Map(u => u.LinkYouTube, "link_youtube");
            Map(u => u.LinkVK, "link_vk");
            Map(u => u.ReviewDate, "review_date");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
