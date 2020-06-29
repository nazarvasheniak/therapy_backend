using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ArticleCommentMap : ClassMap<ArticleComment>
    {
        public ArticleCommentMap()
        {
            Table("articles_comments");
            Id(u => u.ID, "id");

            References(e => e.Author, "id_author");
            References(e => e.Article, "id_article");
            References(e => e.ParentComment, "id_parent_comment");

            Map(u => u.Text, "comment_text").CustomSqlType("text").Length(65535);
            Map(u => u.IsReply, "is_reply");
            Map(u => u.Date, "comment_date");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
