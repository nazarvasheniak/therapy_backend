using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ArticleLikeMap : ClassMap<ArticleLike>
    {
        public ArticleLikeMap()
        {
            Table("articles_likes");
            Id(u => u.ID, "id");

            References(e => e.Author, "id_author");
            References(e => e.Article, "id_article");

            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
