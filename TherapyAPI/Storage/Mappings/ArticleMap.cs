using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ArticleMap : ClassMap<Article>
    {
        public ArticleMap()
        {
            Table("articles");
            Id(u => u.ID, "id");

            References(e => e.Image, "id_image");
            References(e => e.Author, "id_author");

            Map(u => u.Title, "title");
            Map(u => u.ShortText, "short_text").CustomSqlType("text").Length(65535);
            Map(u => u.Text, "full_text").CustomSqlType("text").Length(1994967295);
            Map(u => u.Date, "publish_date");
            Map(u => u.ModerationStatus, "moder_status").CustomType<ArticleModerationStatus>();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
