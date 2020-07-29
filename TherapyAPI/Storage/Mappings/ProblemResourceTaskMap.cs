using System;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class ProblemResourceTaskMap : ClassMap<ProblemResourceTask>
    {
        public ProblemResourceTaskMap()
        {
            Table("problems_resources_tasks");
            Id(u => u.ID, "id");

            References(e => e.Resource, "id_resource");

            Map(u => u.Title, "task_title");
            Map(u => u.IsDone, "is_done");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
