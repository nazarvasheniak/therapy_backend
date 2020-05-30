using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class FileMap : ClassMap<File>
    {
        public FileMap()
        {
            Table("files");
            Id(u => u.ID, "id");

            Map(u => u.Name, "name");
            Map(u => u.Type, "file_type").CustomType<FileType>();
            Map(u => u.LocalPath, "local_path");
            Map(u => u.Url, "url");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
