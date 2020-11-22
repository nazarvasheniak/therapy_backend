using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ArticlePublishMap : ClassMap<ArticlePublish>
    {
        public ArticlePublishMap()
        {
            Table("articles_publish");
            Id(u => u.ID, "id");

            References(e => e.Article, "id_article");

            Map(u => u.Status, "moder_status").CustomType<ArticleModerationStatus>();
            Map(u => u.Message, "moder_message").CustomSqlType("text").Length(65535);
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
